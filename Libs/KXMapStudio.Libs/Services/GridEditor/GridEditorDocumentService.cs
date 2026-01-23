namespace KXMapStudio.Libs.Services.GridEditor;

public sealed class GridEditorDocumentService(
	ISaveFileDialogService saveFileDialogService,
	IJsonService jsonService)
	: IGridEditorDocumentService
{
	private readonly IJsonService _jsonService = jsonService ??
	                                             throw new ArgumentNullException(nameof(jsonService));

	private readonly ISaveFileDialogService _saveFileDialogService = saveFileDialogService ??
	                                                                 throw new ArgumentNullException(
		                                                                 nameof(saveFileDialogService));

	public async Task<(IReadOnlyList<GridEditorRowViewModel> rows, byte[] originalBytes)> LoadAsync(
		EditorDocumentReference doc,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(doc);

		return doc.Extension.ToLowerInvariant() switch
		{
			".json" => await LoadJsonAsync(doc, cancellationToken).ConfigureAwait(false),
			_ => ([], [])
		};
	}

	public async Task SaveAsync(
		EditorDocumentReference doc,
		IReadOnlyList<GridEditorRowViewModel> rows,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(doc);
		ArgumentNullException.ThrowIfNull(rows);

		if (doc.IsArchiveEntry)
			throw new InvalidOperationException("Save is not supported for archive entries. Use Save As instead.");

		ArgumentException.ThrowIfNullOrWhiteSpace(doc.FilePath);

		switch (doc.Extension.ToLowerInvariant())
		{
			case ".json":
				await SaveJsonToPathAsync(rows, doc.FilePath!, _jsonService, cancellationToken).ConfigureAwait(false);
				break;
			default:
				throw new NotSupportedException($"Unsupported file type '{doc.Extension}'.");
		}
	}

	public async Task SaveAsAsync(
		EditorDocumentReference doc,
		IReadOnlyList<GridEditorRowViewModel> rows,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(doc);
		ArgumentNullException.ThrowIfNull(rows);

		switch (doc.Extension.ToLowerInvariant())
		{
			case ".json":
			{
				var targetPath = await _saveFileDialogService.ShowSaveJsonAsync(doc.DisplayName, cancellationToken)
					.ConfigureAwait(false);
				if (string.IsNullOrWhiteSpace(targetPath))
					return;

				await SaveJsonToPathAsync(rows, targetPath, _jsonService, cancellationToken).ConfigureAwait(false);
				break;
			}
			default:
				throw new NotSupportedException($"Unsupported file type '{doc.Extension}'.");
		}
	}


	private async Task<(IReadOnlyList<GridEditorRowViewModel> rows, byte[] originalBytes)> LoadJsonAsync(
		EditorDocumentReference doc,
		CancellationToken cancellationToken)
	{
		// Keep an original snapshot for dirty comparison; Core JSON loader is tolerant/validated.
		var bytes = await LoadBytesAsync(doc, cancellationToken).ConfigureAwait(false);
		if (bytes.Length == 0)
			return ([], bytes);

		// For workspace files, use Core JsonService to honor existing JsonModel schema.
		if (doc.IsWorkspaceFile)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(doc.FilePath);
			var model = await _jsonService.LoadAsync(doc.FilePath!, cancellationToken).ConfigureAwait(false);

			var rows = new List<GridEditorRowViewModel>(model.Coordinates.Length);
			var id = 1;
			foreach (var c in model.Coordinates)
				rows.Add(new GridEditorRowViewModel
				{
					Id = id++,
					Name = c.Name,
					X = c.X,
					Y = c.Y,
					Z = c.Z
				});

			return (rows, bytes);
		}

		// Archive JSON editing: supported for load + SaveAs (export). Parse minimal schema from bytes.
		var fallback = JsonSerializer.Deserialize<CoordinatesModel[]>(bytes,
			new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true,
				NumberHandling = JsonNumberHandling.AllowReadingFromString
			}) ?? [];

		{
			var rows = new List<GridEditorRowViewModel>(fallback.Length);
			var id = 1;
			foreach (var c in fallback)
				rows.Add(new GridEditorRowViewModel { Id = id++, Name = c.Name, X = c.X, Y = c.Y, Z = c.Z });
			return (rows, bytes);
		}
	}

	private async Task<byte[]> LoadBytesAsync(EditorDocumentReference doc, CancellationToken cancellationToken)
	{
		if (doc.IsWorkspaceFile)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(doc.FilePath);
			return await File.ReadAllBytesAsync(doc.FilePath!, cancellationToken).ConfigureAwait(false);
		}

		ArgumentException.ThrowIfNullOrWhiteSpace(doc.ArchivePath);
		ArgumentException.ThrowIfNullOrWhiteSpace(doc.ArchiveEntryFullName);
		return await ReadArchiveEntryBytesAsync(doc.ArchivePath!, doc.ArchiveEntryFullName!, cancellationToken)
			.ConfigureAwait(false);
	}


	private static async Task SaveJsonToPathAsync(
		IReadOnlyList<GridEditorRowViewModel> rows,
		string filePath,
		IJsonService jsonService,
		CancellationToken cancellationToken)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

		// Preserve JsonModel schema. Keep Name based on filename to avoid losing metadata.
		var name = Path.GetFileNameWithoutExtension(filePath);
		var model = new JsonModel(
			name,
			null,
			rows.Select(r => new CoordinatesModel(r.Name, r.X, r.Y, r.Z)).ToArray());

		await jsonService.SaveAsync(model, filePath, cancellationToken).ConfigureAwait(false);
	}

	private static async Task<byte[]> ReadArchiveEntryBytesAsync(string archivePath, string entryFullName,
		CancellationToken cancellationToken)
	{
		await using var zip = await ZipFile.OpenReadAsync(archivePath, cancellationToken);
		var entry = zip.GetEntry(entryFullName);
		if (entry is null)
			return [];

		await using var stream = entry.Open();
		await using var ms = new MemoryStream();
		await stream.CopyToAsync(ms, cancellationToken).ConfigureAwait(false);
		return ms.ToArray();
	}
}