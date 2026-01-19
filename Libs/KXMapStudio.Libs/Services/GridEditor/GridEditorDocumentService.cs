namespace KXMapStudio.Libs.Services.GridEditor;

public sealed class GridEditorDocumentService(
	ISaveFileDialogService saveFileDialogService,
	IFileReaderService fileReaderService,
	IJsonService jsonService)
	: IGridEditorDocumentService
{
	private readonly IFileReaderService _fileReaderService = fileReaderService ??
	                                                         throw new ArgumentNullException(nameof(fileReaderService));

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
			".xml" => await LoadXmlAsync(doc, cancellationToken).ConfigureAwait(false),
			".json" => await LoadJsonAsync(doc, cancellationToken).ConfigureAwait(false),
			_ => ([], Array.Empty<byte>())
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
			case ".xml":
				await SaveXmlToPathAsync(rows, doc.FilePath!, cancellationToken).ConfigureAwait(false);
				break;
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
			case ".xml":
			{
				var targetPath = await _saveFileDialogService.ShowSaveXmlAsync(doc.DisplayName, cancellationToken)
					.ConfigureAwait(false);
				if (string.IsNullOrWhiteSpace(targetPath))
					return;

				await SaveXmlToPathAsync(rows, targetPath, cancellationToken).ConfigureAwait(false);
				break;
			}
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

	private async Task<(IReadOnlyList<GridEditorRowViewModel> rows, byte[] originalBytes)> LoadXmlAsync(
		EditorDocumentReference doc,
		CancellationToken cancellationToken)
	{
		var bytes = await LoadBytesAsync(doc, cancellationToken).ConfigureAwait(false);
		if (bytes.Length == 0)
			return ([], bytes);

		await using var ms = new MemoryStream(bytes, false);
		using var reader = new StreamReader(ms, true);
		var xdoc = await XDocument.LoadAsync(reader, LoadOptions.None, cancellationToken).ConfigureAwait(false);

		var pois = xdoc
			.Descendants()
			.FirstOrDefault(e => e.Name.LocalName.Equals("POIs", StringComparison.OrdinalIgnoreCase));

		if (pois is null)
			return ([], bytes);

		var rows = new List<GridEditorRowViewModel>();
		var id = 1;

		foreach (var poi in pois.Elements()
			         .Where(e => e.Name.LocalName.Equals("POI", StringComparison.OrdinalIgnoreCase)))
		{
			var name = (string?)poi.Attribute("Name") ?? string.Empty;
			var x = ParseDoubleAttribute(poi, "X");
			var y = ParseDoubleAttribute(poi, "Y");
			var z = ParseDoubleAttribute(poi, "Z");

			rows.Add(new GridEditorRowViewModel
			{
				Id = id++,
				Name = name,
				X = x,
				Y = y,
				Z = z
			});
		}

		return (rows, bytes);
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
			}) ?? Array.Empty<CoordinatesModel>();

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

	private static double ParseDoubleAttribute(XElement element, string attributeName)
	{
		var raw = (string?)element.Attribute(attributeName);
		if (string.IsNullOrWhiteSpace(raw))
			return 0d;

		return double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var value)
			? value
			: 0d;
	}

	private static async Task SaveXmlToPathAsync(
		IReadOnlyList<GridEditorRowViewModel> rows,
		string filePath,
		CancellationToken cancellationToken)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

		// TacO structure: <OverlayData><POIs><POI .../></POIs></OverlayData>
		var doc = new XDocument(
			new XDeclaration("1.0", "utf-8", null),
			new XElement("OverlayData", new XElement("POIs")));

		var pois = doc.Root!.Element("POIs")!;

		foreach (var r in rows)
			pois.Add(new XElement(
				"POI",
				new XAttribute("Name", r.Name),
				new XAttribute("X", r.X.ToString("F4", CultureInfo.InvariantCulture)),
				new XAttribute("Y", r.Y.ToString("F4", CultureInfo.InvariantCulture)),
				new XAttribute("Z", r.Z.ToString("F4", CultureInfo.InvariantCulture))));

		await using var stream = File.Create(filePath);
		await doc.SaveAsync(stream, SaveOptions.DisableFormatting, cancellationToken).ConfigureAwait(false);
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
		using var zip = ZipFile.OpenRead(archivePath);
		var entry = zip.GetEntry(entryFullName);
		if (entry is null)
			return Array.Empty<byte>();

		await using var stream = entry.Open();
		await using var ms = new MemoryStream();
		await stream.CopyToAsync(ms, cancellationToken).ConfigureAwait(false);
		return ms.ToArray();
	}
}