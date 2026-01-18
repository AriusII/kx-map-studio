namespace KXMapStudio.Libs.Services;

/// <summary>
///     UI-facing explorer service (WPF). Delegates filesystem/tree construction to Core and maps to WPF models.
/// </summary>
public sealed class WorkshopExplorerService : IWorkshopExplorerService
{
	private readonly IFileExplorerService _coreExplorer;
	private readonly IFileExplorerNodeService _nodeService;

	public WorkshopExplorerService(IFileExplorerService coreExplorer, IFileExplorerNodeService nodeService)
	{
		_coreExplorer = coreExplorer ?? throw new ArgumentNullException(nameof(coreExplorer));
		_nodeService = nodeService ?? throw new ArgumentNullException(nameof(nodeService));

		DataFolder = Path.Combine(AppContext.BaseDirectory, Constants.Settings.DataFolder);
		Directory.CreateDirectory(DataFolder);
	}

	public string DataFolder { get; }

	public WorkspaceExplorerNodeModel BuildRootNode(bool recursive = true)
	{
		var coreRoot = _coreExplorer.BuildTree(recursive);
		return Map(coreRoot);
	}

	public WorkspaceExplorerNodeModel? FindNodeByPath(WorkspaceExplorerNodeModel nodeModel, string fullPath)
	{
		return _nodeService.FindByPath(nodeModel, fullPath);
	}

	public bool IsRelevantChange(string fullPath)
	{
		if (string.IsNullOrWhiteSpace(fullPath))
			return false;

		try
		{
			var normalized = Path.GetFullPath(fullPath);
			var dataRoot = Path.GetFullPath(DataFolder);

			return normalized.StartsWith(dataRoot, StringComparison.OrdinalIgnoreCase);
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