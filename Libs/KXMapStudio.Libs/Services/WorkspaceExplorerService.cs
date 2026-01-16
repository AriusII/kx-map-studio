namespace KXMapStudio.Libs.Services;

public sealed class WorkspaceExplorerService : IWorkspaceExplorerService
{
	public WorkspaceExplorerService(string? dataFolder = null)
	{
		DataFolder = dataFolder ?? Path.Combine(AppContext.BaseDirectory, Constants.Settings.DataFolder);
		Directory.CreateDirectory(DataFolder);
	}

	public string DataFolder { get; }

	public WorkspaceExplorerNodeModel BuildRootNode()
	{
		var dataDirectory = new DirectoryInfo(DataFolder);
		var children = BuildNodes(dataDirectory).ToList();

		return new WorkspaceExplorerNodeModel(
			dataDirectory.Name,
			dataDirectory.FullName,
			true,
			new ObservableCollection<WorkspaceExplorerNodeModel>(children));
	}

	public WorkspaceExplorerNodeModel? FindNodeByPath(WorkspaceExplorerNodeModel nodeModel, string fullPath)
	{
		if (string.Equals(nodeModel.FullPath, fullPath, StringComparison.OrdinalIgnoreCase)) return nodeModel;

		foreach (var child in nodeModel.Children)
		{
			var match = FindNodeByPath(child, fullPath);
			if (match is null) continue;

			if (nodeModel.IsDirectory) nodeModel.IsExpanded = true;

			return match;
		}

		return null;
	}

	public bool IsRelevantChange(string fullPath)
	{
		return !Directory.Exists(fullPath) && IsAllowedFilePath(fullPath);
	}

	public bool IsAllowedFilePath(string fullPath)
	{
		var extension = Path.GetExtension(fullPath);
		return FileExtension.AllowedExtensions.Contains(extension);
	}

	private static IEnumerable<WorkspaceExplorerNodeModel> BuildNodes(DirectoryInfo directory)
	{
		var nodes = new List<WorkspaceExplorerNodeModel>();

		foreach (var subDirectory in EnumerateDirectoriesSafe(directory))
		{
			var childNodes = BuildNodes(subDirectory).ToList();

			if (childNodes.Count > 0)
				nodes.Add(new WorkspaceExplorerNodeModel(
					subDirectory.Name,
					subDirectory.FullName,
					true,
					new ObservableCollection<WorkspaceExplorerNodeModel>(childNodes)));
		}

		foreach (var file in EnumerateFilesSafe(directory).Where(IsAllowedFile))
			nodes.Add(new WorkspaceExplorerNodeModel(file.Name, file.FullName, false));

		return nodes;
	}

	private static IEnumerable<DirectoryInfo> EnumerateDirectoriesSafe(DirectoryInfo directory)
	{
		try
		{
			return directory.EnumerateDirectories();
		}
		catch
		{
			return [];
		}
	}

	private static IEnumerable<FileInfo> EnumerateFilesSafe(DirectoryInfo directory)
	{
		try
		{
			return directory.EnumerateFiles();
		}
		catch
		{
			return [];
		}
	}

	private static bool IsAllowedFile(FileInfo file)
	{
		return FileExtension.AllowedExtensions.Contains(file.Extension);
	}
}