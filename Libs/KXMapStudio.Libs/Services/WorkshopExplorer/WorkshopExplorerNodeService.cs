namespace KXMapStudio.Libs.Services.WorkshopExplorer;

/// <summary>
///     Consolidated tree manipulation service for Workshop Explorer.
///     Handles node traversal, expansion, selection, and tree building operations.
/// </summary>
public sealed class WorkshopExplorerNodeService : IWorkshopExplorerNodeService
{
	/// <summary>
	///     Finds a node in the tree by its full path.
	/// </summary>
	public WorkspaceExplorerNodeModel? FindByPath(WorkspaceExplorerNodeModel root, string fullPath)
	{
		ArgumentNullException.ThrowIfNull(root);
		if (string.IsNullOrWhiteSpace(fullPath))
			return null;

		var normalized = NormalizeFullPath(fullPath);
		return FindInternal(root, normalized);
	}

	/// <summary>
	///     Expands all parent nodes in the tree leading to the target path.
	/// </summary>
	public bool ExpandParents(WorkspaceExplorerNodeModel current, string targetFullPath)
	{
		if (PathEquals(current.FullPath, targetFullPath))
			return true;

		foreach (var child in current.Children)
		{
			if (!ExpandParents(child, targetFullPath))
				continue;

			current.IsExpanded = true;
			return true;
		}

		return false;
	}

	/// <summary>
	///     Traverses the tree and applies an action to each node.
	/// </summary>
	public void TraverseTree(WorkspaceExplorerNodeModel node, Action<WorkspaceExplorerNodeModel> action)
	{
		ArgumentNullException.ThrowIfNull(node);
		ArgumentNullException.ThrowIfNull(action);

		action(node);
		foreach (var child in node.Children)
			TraverseTree(child, action);
	}

	/// <summary>
	///     Maps a scan result node tree to UI node tree.
	/// </summary>
	public WorkspaceExplorerNodeModel MapScanNodeToUiNode(WorkshopExplorerScanNode scanNode)
	{
		ArgumentNullException.ThrowIfNull(scanNode);

		var ui = new WorkspaceExplorerNodeModel(scanNode.Name, scanNode.FullPath, scanNode.IsDirectory);
		foreach (var child in scanNode.Children)
			ui.Children.Add(MapScanNodeToUiNode(child));
		return ui;
	}

	/// <summary>
	///     Normalizes a file path to its full absolute path.
	/// </summary>
	public static string NormalizeFullPath(string path)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(path);
		return Path.GetFullPath(path);
	}

	/// <summary>
	///     Checks if two paths are equal using case-insensitive comparison.
	/// </summary>
	public static bool PathEquals(string path1, string path2)
	{
		return string.Equals(path1, path2, StringComparison.OrdinalIgnoreCase);
	}

	/// <summary>
	///     Checks if a path starts with another path (case-insensitive).
	/// </summary>
	public static bool PathStartsWith(string path, string prefix)
	{
		return path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
	}

	private static WorkspaceExplorerNodeModel? FindInternal(WorkspaceExplorerNodeModel node, string fullPath)
	{
		if (PathEquals(node.FullPath, fullPath))
			return node;

		foreach (var child in node.Children)
		{
			var found = FindInternal(child, fullPath);
			if (found != null)
				return found;
		}

		return null;
	}
}