namespace KXMapStudio.Libs.Services.WorkshopExplorer;

/// <summary>
///     UI-only tree helpers (selection + expansion).
/// </summary>
public sealed record WorkshopExplorerNodeService : IWorkshopExplorerNodeService
{
	public WorkspaceExplorerNodeModel? FindByPath(WorkspaceExplorerNodeModel root, string fullPath)
	{
		ArgumentNullException.ThrowIfNull(root);
		if (string.IsNullOrWhiteSpace(fullPath))
			return null;

		var normalized = Path.GetFullPath(fullPath);
		return FindInternal(root, normalized);
	}

	public bool ExpandParents(WorkspaceExplorerNodeModel current, string targetFullPath)
	{
		if (string.Equals(current.FullPath, targetFullPath, StringComparison.OrdinalIgnoreCase))
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

	private static WorkspaceExplorerNodeModel? FindInternal(WorkspaceExplorerNodeModel node, string fullPath)
	{
		if (string.Equals(node.FullPath, fullPath, StringComparison.OrdinalIgnoreCase))
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