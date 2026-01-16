namespace KXMapStudio.Libs.Abstractions;

public interface IWorkspaceExplorerService
{
	string DataFolder { get; }
	WorkspaceExplorerNodeModel BuildRootNode();
	WorkspaceExplorerNodeModel? FindNodeByPath(WorkspaceExplorerNodeModel nodeModel, string fullPath);
	bool IsRelevantChange(string fullPath);
	bool IsAllowedFilePath(string fullPath);
}