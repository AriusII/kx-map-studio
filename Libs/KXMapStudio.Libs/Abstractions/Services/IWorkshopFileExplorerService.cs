namespace KXMapStudio.Libs.Abstractions.Services;

public interface IWorkshopFileExplorerService
{
	string DataFolder { get; }
	WorkspaceExplorerNodeModel BuildRootNode();
	WorkspaceExplorerNodeModel? FindNodeByPath(WorkspaceExplorerNodeModel nodeModel, string fullPath);
	bool IsRelevantChange(string fullPath);
	bool IsAllowedFilePath(string fullPath);
}