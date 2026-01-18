namespace KXMapStudio.Libs.Abstractions.Services;

public interface IFileExplorerNodeService
{
	WorkspaceExplorerNodeModel? FindByPath(WorkspaceExplorerNodeModel root, string fullPath);
	bool ExpandParents(WorkspaceExplorerNodeModel current, string targetFullPath);
}