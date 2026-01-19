namespace KXMapStudio.Libs.Abstractions.Services.WorkshopExplorer;

public interface IWorkshopExplorerService
{
	string DataFolder { get; }
	WorkspaceExplorerNodeModel BuildRootNode(bool recursive = true);
	WorkspaceExplorerNodeModel? FindNodeByPath(WorkspaceExplorerNodeModel nodeModel, string fullPath);
	bool IsRelevantChange(string fullPath);
	bool IsAllowedFilePath(string fullPath);
}