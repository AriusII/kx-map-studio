namespace KXMapStudio.Core.Services;

/// <summary>
///     Enumerates entries under the application data root folder.
/// </summary>
public sealed record FileExplorerService : IFileExplorerService
{
	public IEnumerable<FileSystemEntryModel> EnumerateEntries(bool recursive = true)
	{
		return EnumerateFrom(GetDataRootPath(), recursive);
	}

	public FileSystemEntryNodeModel BuildTree(bool recursive = true)
	{
		var dataRoot = Path.GetFullPath(GetDataRootPath());
		Directory.CreateDirectory(dataRoot);

		var entries = EnumerateEntries(recursive)
			.Where(e => e.Type != ExplorerEntryType.Root)
			.ToList();

		var root = new FileSystemEntryNodeModel(
			Path.GetFileName(dataRoot) is { Length: > 0 } n ? n : dataRoot,
			dataRoot,
			ExplorerEntryType.Root
		);

		var nodes = new Dictionary<string, FileSystemEntryNodeModel>(StringComparer.OrdinalIgnoreCase)
		{
			[dataRoot] = root
		};

		foreach (var entry in entries)
		{
			var fullPath = Path.GetFullPath(entry.FullPath);
			nodes[fullPath] = new FileSystemEntryNodeModel(entry.Name, fullPath, entry.Type);
		}

		foreach (var (path, node) in nodes.ToList())
		{
			if (ReferenceEquals(node, root))
				continue;

			var parentPath = Path.GetDirectoryName(path);
			if (string.IsNullOrWhiteSpace(parentPath))
				continue;

			parentPath = Path.GetFullPath(parentPath);

			if (!nodes.TryGetValue(parentPath, out var parent))
			{
				var parentName = Path.GetFileName(parentPath) is { Length: > 0 } pn ? pn : parentPath;
				parent = new FileSystemEntryNodeModel(parentName, parentPath, ExplorerEntryType.Folder);
				nodes[parentPath] = parent;
			}

			if (!parent.Children.Any(c => string.Equals(c.FullPath, node.FullPath, StringComparison.OrdinalIgnoreCase)))
				parent.Children.Add(node);
		}

		SortRecursively(root);
		return root;
	}

	private static void SortRecursively(FileSystemEntryNodeModel node)
	{
		if (node.Children.Count == 0)
			return;

		var sorted = node.Children
			.OrderBy(c => Rank(c.Type))
			.ThenBy(c => c.Name, StringComparer.OrdinalIgnoreCase)
			.ToList();

		node.Children.Clear();
		node.Children.AddRange(sorted);

		foreach (var child in node.Children)
			SortRecursively(child);
		return;

		static int Rank(ExplorerEntryType t)
		{
			return t switch
			{
				ExplorerEntryType.Root => 0,
				ExplorerEntryType.Folder => 1,
				ExplorerEntryType.Archive => 2,
				ExplorerEntryType.File => 3,
				_ => 9
			};
		}
	}

	private static string GetDataRootPath()
	{
		return Path.Combine(AppContext.BaseDirectory, Constants.Settings.DataFolder);
	}

	private static IEnumerable<FileSystemEntryModel> EnumerateFrom(string startPath, bool recursive = true)
	{
		if (string.IsNullOrWhiteSpace(startPath))
			yield break;

		var normalizedStartPath = Path.GetFullPath(startPath);
		if (!Directory.Exists(normalizedStartPath))
			yield break;

		yield return new FileSystemEntryModel(
			ExplorerEntryType.Root,
			normalizedStartPath,
			Path.GetFileName(normalizedStartPath) is { Length: > 0 } n ? n : normalizedStartPath,
			".",
			string.Empty
		);

		var dirs = new Stack<string>();
		dirs.Push(normalizedStartPath);

		while (dirs.Count > 0)
		{
			var current = dirs.Pop();

			IEnumerable<string> subDirs;
			try
			{
				subDirs = Directory.EnumerateDirectories(current);
			}
			catch (UnauthorizedAccessException)
			{
				continue;
			}
			catch (DirectoryNotFoundException)
			{
				continue;
			}

			foreach (var d in subDirs)
			{
				var rel = Path.GetRelativePath(normalizedStartPath, d);
				yield return new FileSystemEntryModel(
					ExplorerEntryType.Folder,
					d,
					Path.GetFileName(d),
					rel,
					string.Empty
				);

				if (recursive)
					dirs.Push(d);
			}

			IEnumerable<string> files;
			try
			{
				files = Directory.EnumerateFiles(current);
			}
			catch (UnauthorizedAccessException)
			{
				continue;
			}
			catch (DirectoryNotFoundException)
			{
				continue;
			}

			foreach (var f in files)
			{
				var ext = Path.GetExtension(f);
				if (!FileExtension.AllowedExtensions.Contains(ext))
					continue;

				var type = IsArchiveExtension(ext) ? ExplorerEntryType.Archive : ExplorerEntryType.File;
				var rel = Path.GetRelativePath(normalizedStartPath, f);

				yield return new FileSystemEntryModel(
					type,
					f,
					Path.GetFileName(f),
					rel,
					ext
				);
			}
		}
	}

	private static bool IsArchiveExtension(string ext)
	{
		return string.Equals(ext, FileExtension.Zip, StringComparison.OrdinalIgnoreCase)
		       || string.Equals(ext, FileExtension.Taco, StringComparison.OrdinalIgnoreCase);
	}
}