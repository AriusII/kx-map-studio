namespace KXMapStudio.Libs.Services.FilePreview;

public sealed record FilePreviewService(IFileReaderService FileReaderService) : IFilePreviewService
{
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

	private async Task<string> PreviewJsonAsync(string fullPath, string fileName, CancellationToken ct)
	{
		if (string.Equals(fileName, "maps.json", StringComparison.OrdinalIgnoreCase))
		{
			var maps = await FileReaderService.ReadMapsJsonAsync(fullPath, ct).ConfigureAwait(false);
			return $"Maps JSON loaded. Items: {maps.Count.ToString(CultureInfo.InvariantCulture)}";
		}

		if (string.Equals(fileName, "continents.json", StringComparison.OrdinalIgnoreCase))
		{
			var continents = await FileReaderService.ReadContinentsJsonAsync(fullPath, ct).ConfigureAwait(false);
			return continents is null ? "Continents JSON loaded. Empty." : "Continents JSON loaded.";
		}

		var json = await FileReaderService.ReadJsonAsync(fullPath, ct).ConfigureAwait(false);
		_ = json;
		return "JSON loaded.";
	}

	private async Task<string> PreviewXmlAsync(string fullPath, CancellationToken ct)
	{
		var taco = await FileReaderService.ReadTacoAsync(fullPath, FileType.Xml, ct).ConfigureAwait(false);
		return taco is null ? "XML loaded. Empty." : "XML loaded.";
	}

	private static IReadOnlyList<PreviewTreeNodeModel> BuildArchiveTree(string archivePath)
	{
		var root = new PreviewTreeNodeModel(Path.GetFileName(archivePath), archivePath);
		var folders = new Dictionary<string, PreviewTreeNodeModel>(StringComparer.OrdinalIgnoreCase)
		{
			[""] = root
		};

		using var zip = ZipFile.OpenRead(archivePath);

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
					var leaf = new PreviewTreeNodeModel(part, currentPath, null, true);
					parent.Children.Add(leaf);
					continue;
				}

				if (!folders.TryGetValue(currentPath, out var folderNode))
				{
					folderNode = new PreviewTreeNodeModel(part, currentPath);
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