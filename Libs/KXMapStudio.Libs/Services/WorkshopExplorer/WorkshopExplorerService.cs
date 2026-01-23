namespace KXMapStudio.Libs.Services.WorkshopExplorer;

/// <summary>
///     Workshop Explorer service - handles filesystem scanning, tree building, and node operations.
/// </summary>
public sealed class WorkshopExplorerService : IWorkshopExplorerService
{
	private static readonly IReadOnlyCollection<string> AllowedWorkshopExtensions =
	[
		FileExtension.Json
	];

	public WorkshopExplorerService()
	{
		DataFolder = Path.Combine(AppContext.BaseDirectory, Constants.Settings.DataFolder);
		Directory.CreateDirectory(DataFolder);
	}

	public string DataFolder { get; }

	public bool IsRelevantChange(string fullPath)
	{
		// Ignore temp files and irrelevant changes
		if (string.IsNullOrWhiteSpace(fullPath))
			return false;

		var fileName = Path.GetFileName(fullPath);
		if (fileName.StartsWith('.') || fileName.StartsWith('~'))
			return false;

		return IsAllowedFilePath(fullPath);
	}

	public bool IsAllowedFilePath(string fullPath)
	{
		if (string.IsNullOrWhiteSpace(fullPath))
			return false;

		// Allow directories
		if (Directory.Exists(fullPath))
			return true;

		// Filter files by extension
		var extension = Path.GetExtension(fullPath);
		return AllowedWorkshopExtensions.Contains(extension);
	}

	public async Task<WorkshopExplorerScanNode> ScanDirectoryAsync(string directoryPath,
		CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(directoryPath);

		return await Task.Run(() => ScanDirectoryRecursive(directoryPath, cancellationToken), cancellationToken);
	}

	private WorkshopExplorerScanNode ScanDirectoryRecursive(string directoryPath, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		var dirInfo = new DirectoryInfo(directoryPath);
		var children = new List<WorkshopExplorerScanNode>();

		// Scan subdirectories first
		try
		{
			foreach (var subDir in dirInfo.EnumerateDirectories()
				         .OrderBy(d => d.Name, StringComparer.OrdinalIgnoreCase))
			{
				cancellationToken.ThrowIfCancellationRequested();

				var subDirNode = ScanDirectoryRecursive(subDir.FullName, cancellationToken);

				// Only include directories that have XML/JSON files (or subdirectories with such files)
				if (subDirNode.Children.Count > 0)
					children.Add(subDirNode);
			}
		}
		catch (UnauthorizedAccessException)
		{
			// Skip directories we can't access
		}

		// Scan files (only XML and JSON)
		try
		{
			foreach (var file in dirInfo.EnumerateFiles().OrderBy(f => f.Name, StringComparer.OrdinalIgnoreCase))
			{
				cancellationToken.ThrowIfCancellationRequested();

				var extension = file.Extension.ToLowerInvariant();
				if (AllowedWorkshopExtensions.Contains(extension))
					children.Add(new WorkshopExplorerScanNode(
						file.Name,
						file.FullName,
						false,
						Array.Empty<WorkshopExplorerScanNode>()));
			}
		}
		catch (UnauthorizedAccessException)
		{
			// Skip files we can't access
		}

		return new WorkshopExplorerScanNode(
			dirInfo.Name,
			dirInfo.FullName,
			true,
			children);
	}
}