namespace KXMapStudio.Libs.Services;

/// <summary>
///     UI-facing explorer service (WPF). Delegates filesystem/tree construction to Core and maps to WPF models.
/// </summary>
public sealed class WorkshopFileExplorerService : IWorkshopFileExplorerService
{
	private readonly IFileExplorerService _coreExplorer;
	private readonly IFileExplorerNodeService _fileExplorerNodeService;

	public WorkshopFileExplorerService(IFileExplorerService coreExplorer,
		IFileExplorerNodeService fileExplorerNodeService)
	{
		_coreExplorer = coreExplorer ?? throw new ArgumentNullException(nameof(coreExplorer));
		_fileExplorerNodeService =
			fileExplorerNodeService ?? throw new ArgumentNullException(nameof(fileExplorerNodeService));

		DataFolder = Path.Combine(AppContext.BaseDirectory, Constants.Settings.DataFolder);
		Directory.CreateDirectory(DataFolder);
	}

	public string DataFolder { get; }

	public WorkspaceExplorerNodeModel BuildRootNode()
	{
		var coreRoot = _coreExplorer.BuildTree();
		return Map(coreRoot);
	}

	public WorkspaceExplorerNodeModel? FindNodeByPath(WorkspaceExplorerNodeModel nodeModel, string fullPath)
	{
		return _fileExplorerNodeService.FindByPath(nodeModel, fullPath);
	}

	public bool IsRelevantChange(string fullPath)
	{
		if (string.IsNullOrWhiteSpace(fullPath))
			return false;

		try
		{
			var normalized = Path.GetFullPath(fullPath);
			if (!normalized.StartsWith(Path.GetFullPath(DataFolder), StringComparison.OrdinalIgnoreCase))
				return false;

			return Directory.Exists(normalized) || IsAllowedFilePath(normalized);
		}
		catch
		{
			return false;
		}
	}

	public bool IsAllowedFilePath(string fullPath)
	{
		if (string.IsNullOrWhiteSpace(fullPath))
			return false;

		var ext = Path.GetExtension(fullPath);
		return FileExtension.AllowedExtensions.Contains(ext);
	}

	private static WorkspaceExplorerNodeModel Map(FileSystemEntryNodeModel coreNode)
	{
		var isDir = coreNode.Type is ExplorerEntryType.Root or ExplorerEntryType.Folder;
		var ui = new WorkspaceExplorerNodeModel(coreNode.Name, coreNode.FullPath, isDir);

		foreach (var child in coreNode.Children)
			ui.Children.Add(Map(child));

		return ui;
	}
}