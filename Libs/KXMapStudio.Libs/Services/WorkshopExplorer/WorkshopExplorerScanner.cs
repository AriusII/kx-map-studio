namespace KXMapStudio.Libs.Services.WorkshopExplorer;

/// <summary>
///     Scans the filesystem for folders and allowed file types.
/// </summary>
public sealed class WorkshopExplorerScanner : IWorkshopExplorerScanner
{
	public Task<WorkshopExplorerScanNode> ScanAsync(
		string rootPath,
		IReadOnlyCollection<string> allowedExtensions,
		CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(rootPath))
			throw new ArgumentException("Root path is required.", nameof(rootPath));
		ArgumentNullException.ThrowIfNull(allowedExtensions);

		var rootFullPath = WorkshopExplorerNodeService.NormalizeFullPath(rootPath);
		var allowed = new HashSet<string>(allowedExtensions, WorkshopExplorerConstants.PathComparer);

		// Ensure the folder exists; if it doesn't, return an empty root.
		if (Directory.Exists(rootFullPath))
			return Task.Run(() => BuildDirectoryNode(rootFullPath, allowed, cancellationToken), cancellationToken);
		var name = Path.GetFileName(rootFullPath.TrimEnd(Path.DirectorySeparatorChar,
			Path.AltDirectorySeparatorChar));
		if (string.IsNullOrWhiteSpace(name))
			name = rootFullPath;

		return Task.FromResult(new WorkshopExplorerScanNode(name, rootFullPath, true, []));
	}

	private static WorkshopExplorerScanNode BuildDirectoryNode(string directoryPath, HashSet<string> allowed,
		CancellationToken ct)
	{
		ct.ThrowIfCancellationRequested();

		var name = Path.GetFileName(directoryPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
		if (string.IsNullOrWhiteSpace(name))
			name = directoryPath;

		var children = new List<WorkshopExplorerScanNode>();

		try
		{
			foreach (var dir in Directory.EnumerateDirectories(directoryPath))
			{
				ct.ThrowIfCancellationRequested();

				if (ShouldSkipDirectory(dir))
					continue;

				var childDirNode = BuildDirectoryNode(dir, allowed, ct);
				children.Add(childDirNode);
			}

			foreach (var file in Directory.EnumerateFiles(directoryPath))
			{
				ct.ThrowIfCancellationRequested();

				var ext = Path.GetExtension(file);
				if (!allowed.Contains(ext))
					continue;

				children.Add(new WorkshopExplorerScanNode(Path.GetFileName(file), Path.GetFullPath(file), false, []));
			}
		}
		catch (UnauthorizedAccessException)
		{
			// Ignore protected folders: they shouldn't break the explorer.
		}
		catch (DirectoryNotFoundException)
		{
			// Folder may disappear during scan.
		}

		// Sort: directories first, then files. Both alphabetical, case-insensitive.
		children.Sort(static (a, b) =>
		{
			if (a.IsDirectory != b.IsDirectory)
				return a.IsDirectory ? -1 : 1;

			return WorkshopExplorerConstants.PathComparer.Compare(a.Name, b.Name);
		});

		return new WorkshopExplorerScanNode(name, Path.GetFullPath(directoryPath), true, children);
	}

	private static bool ShouldSkipDirectory(string directoryPath)
	{
		try
		{
			var di = new DirectoryInfo(directoryPath);
			if ((di.Attributes & FileAttributes.Hidden) != 0)
				return true;
			return (di.Attributes & FileAttributes.System) != 0;
		}
		catch
		{
			// If we can't read attributes, don't skip it.
			return false;
		}
	}
}