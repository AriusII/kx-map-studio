namespace KXMapStudio.Libs.Services.FilePreview;

public sealed class FilePreviewService : IFilePreviewService, IDisposable
{
	private readonly Dictionary<string, ZipArchive> _openArchives = new(StringComparer.OrdinalIgnoreCase);

	/// <summary>
	///     Initializes a new instance of the <see cref="FilePreviewService" />.
	/// </summary>

	public void Dispose()
	{
		foreach (var a in _openArchives.Values)
			a.Dispose();
		_openArchives.Clear();
	}

	/// <inheritdoc />
	public async Task<FilePreviewResult> BuildPreviewAsync(string fullPath,
		CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(fullPath))
			return Empty("No file selected.");

		var sw = Stopwatch.StartNew();

		try
		{
			var ext = Path.GetExtension(fullPath);
			var fileName = Path.GetFileName(fullPath);

			if (IsArchiveExtension(ext))
			{
				var nodes = BuildArchiveTree(fullPath);
				return Finish(fileName, "Archive contents loaded.", sw, nodes);
			}

			if (string.Equals(ext, ".json", StringComparison.OrdinalIgnoreCase))
			{
				var status = await PreviewJsonAsync(fullPath, fileName, cancellationToken).ConfigureAwait(false);
				return Finish(fileName, status, sw, []);
			}

			if (string.Equals(ext, ".xml", StringComparison.OrdinalIgnoreCase))
			{
				var status = await PreviewXmlAsync(fullPath, cancellationToken).ConfigureAwait(false);
				return Finish(fileName, status, sw, []);
			}

			return Finish(fileName, "Unsupported file type.", sw, []);
		}
		catch (OperationCanceledException)
		{
			return Finish(Path.GetFileName(fullPath), "Preview canceled.", sw, []);
		}
		catch (Exception ex)
		{
			return Finish(Path.GetFileName(fullPath), $"Preview failed: {ex.Message}", sw, []);
		}
	}

	/// <summary>
	///     Expands an archive XML entry node into DTO-driven child nodes.
	/// </summary>
	/// <remarks>
	///     This method keeps the underlying archive open for further exploration.
	/// </remarks>
	public async Task<IReadOnlyList<PreviewTreeNodeModel>> ExpandArchiveXmlAsync(PreviewTreeNodeModel archiveXmlNode,
		CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}

	private ZipArchive GetOrOpenArchive(string archivePath)
	{
		if (_openArchives.TryGetValue(archivePath, out var existing))
			return existing;

		var zip = ZipFile.OpenRead(archivePath);
		_openArchives[archivePath] = zip;
		return zip;
	}

	private async Task<string> PreviewJsonAsync(string fullPath, string fileName, CancellationToken ct)
	{
		throw new NotImplementedException();
	}

	private async Task<string> PreviewXmlAsync(string fullPath, CancellationToken ct)
	{
		throw new NotImplementedException();
	}

	private IReadOnlyList<PreviewTreeNodeModel> BuildArchiveTree(string archivePath)
	{
		var root = new PreviewTreeNodeModel(Path.GetFileName(archivePath), archivePath,
			kind: PreviewTreeNodeKind.ArchiveFolder);
		var folders = new Dictionary<string, PreviewTreeNodeModel>(StringComparer.OrdinalIgnoreCase)
		{
			[""] = root
		};

		var zip = GetOrOpenArchive(archivePath);

		foreach (var entry in zip.Entries)
		{
			if (string.IsNullOrWhiteSpace(entry.FullName))
				continue;

			var path = entry.FullName.Replace('\\', '/');
			var isDir = path.EndsWith("/", StringComparison.Ordinal);
			var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

			var currentPath = "";
			var parent = root;

			for (var i = 0; i < parts.Length; i++)
			{
				var part = parts[i];
				var isLast = i == parts.Length - 1;

				currentPath = currentPath.Length == 0 ? part : $"{currentPath}/{part}";

				if (isLast && !isDir)
				{
					var leaf = new PreviewTreeNodeModel(
						part,
						currentPath,
						null,
						true,
						archivePath,
						currentPath,
						PreviewTreeNodeKind.ArchiveEntry);
					parent.Children.Add(leaf);
					continue;
				}

				if (!folders.TryGetValue(currentPath, out var folderNode))
				{
					folderNode = new PreviewTreeNodeModel(part, currentPath, kind: PreviewTreeNodeKind.ArchiveFolder);
					folders[currentPath] = folderNode;
					parent.Children.Add(folderNode);
				}

				parent = folderNode;
			}
		}

		SortRecursively(root);
		UpdateFolderCounts(root);

		root.IsExpanded = true;
		return [root];
	}

	private static void SortRecursively(PreviewTreeNodeModel node)
	{
		if (node.Children.Count == 0)
			return;

		var sorted = node.Children
			.OrderBy(c => c.IsLeaf ? 1 : 0)
			.ThenBy(c => c.Name, StringComparer.OrdinalIgnoreCase)
			.ToList();

		node.Children.Clear();
		foreach (var s in sorted)
			node.Children.Add(s);

		foreach (var child in node.Children)
			SortRecursively(child);
	}

	private static int UpdateFolderCounts(PreviewTreeNodeModel node)
	{
		if (node.Children.Count == 0)
			return 0;

		var leafCount = 0;

		foreach (var child in node.Children)
		{
			if (child.IsLeaf)
			{
				leafCount++;
				continue;
			}

			leafCount += UpdateFolderCounts(child);
		}

		return leafCount;
	}

	private static bool IsArchiveExtension(string ext)
	{
		return string.Equals(ext, ".zip", StringComparison.OrdinalIgnoreCase)
		       || string.Equals(ext, ".taco", StringComparison.OrdinalIgnoreCase);
	}

	private static FilePreviewResult Empty(string status)
	{
		return new FilePreviewResult(
			string.Empty,
			status,
			string.Empty,
			[]);
	}

	private static FilePreviewResult Finish(string openedFileName, string status, Stopwatch sw,
		IReadOnlyList<PreviewTreeNodeModel> nodes)
	{
		sw.Stop();

		return new FilePreviewResult(
			openedFileName,
			status,
			$"Loaded in {sw.ElapsedMilliseconds.ToString(CultureInfo.InvariantCulture)} ms",
			nodes);
	}
}