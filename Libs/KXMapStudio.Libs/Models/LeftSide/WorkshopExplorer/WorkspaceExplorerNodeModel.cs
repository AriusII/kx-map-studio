namespace KXMapStudio.Libs.Models.LeftSide.WorkshopExplorer;

public sealed partial class WorkspaceExplorerNodeModel(
	string name,
	string fullPath,
	bool isDirectory,
	ObservableCollection<WorkspaceExplorerNodeModel> children)
	: ObservableObject
{
	[ObservableProperty] private bool _isExpanded;

	public WorkspaceExplorerNodeModel(string name, string fullPath, bool isDirectory)
		: this(name, fullPath, isDirectory, [])
	{
	}

	public string Name { get; } = name;
	public string FullPath { get; } = fullPath;
	public bool IsDirectory { get; } = isDirectory;
	public ObservableCollection<WorkspaceExplorerNodeModel> Children { get; } = children;
}