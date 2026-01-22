namespace KXMapStudio.Libs.Abstractions.Services.WorkshopExplorer;

/// <summary>
///     Provides tree manipulation operations for Workshop Explorer nodes.
/// </summary>
public interface IWorkshopExplorerNodeService
{
	/// <summary>
	///     Finds a node in the tree by its full path.
	/// </summary>
	WorkspaceExplorerNodeModel? FindByPath(WorkspaceExplorerNodeModel root, string fullPath);

	/// <summary>
	///     Expands all parent nodes leading to the target path.
	/// </summary>
	bool ExpandParents(WorkspaceExplorerNodeModel current, string targetFullPath);

	/// <summary>
	///     Traverses the tree and applies an action to each node.
	/// </summary>
	void TraverseTree(WorkspaceExplorerNodeModel node, Action<WorkspaceExplorerNodeModel> action);

	/// <summary>
	///     Maps a scan result node tree to UI node tree.
	/// </summary>
	WorkspaceExplorerNodeModel MapScanNodeToUiNode(WorkshopExplorerScanNode scanNode);
}