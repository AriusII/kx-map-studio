namespace KXMapStudio.Core.Services;

/// <summary>
///     Enumerates entries under the application data root folder.
/// </summary>
public sealed record FileExplorerService : IFileExplorerService
{
	/// <inheritdoc />
	public IEnumerable<FileSystemEntryModel> EnumerateEntries(bool recursive = true)
	{
		return EnumerateFrom(GetDataRootPath(), recursive);
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