namespace KXMapStudio.Libs.Abstractions.Services.WorkshopExplorer;

public interface IWorkshopExplorerNodeService
{
	WorkspaceExplorerNodeModel? FindByPath(WorkspaceExplorerNodeModel root, string fullPath);
	bool ExpandParents(WorkspaceExplorerNodeModel current, string targetFullPath);
}