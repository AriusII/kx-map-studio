namespace KXMapStudio.Libs.Models.LeftSide.WorkshopExplorer;

/// <summary>
///     UI-bindable tree node for Workshop Explorer.
/// </summary>
public sealed partial class WorkshopExplorerNodeModel : ObservableObject
{
	[ObservableProperty] private bool _isExpanded;
	[ObservableProperty] private bool _isModified;
	[ObservableProperty] private bool _isSelected;

	public WorkshopExplorerNodeModel(string name, string fullPath, bool isDirectory)
	{
		Name = name;
		FullPath = fullPath;
		IsDirectory = isDirectory;
		Extension = isDirectory ? string.Empty : Path.GetExtension(fullPath);
		Children = new ObservableCollection<WorkshopExplorerNodeModel>();
	}

	public string Name { get; }
	public string FullPath { get; }
	public bool IsDirectory { get; }
	public string Extension { get; }
	public ObservableCollection<WorkshopExplorerNodeModel> Children { get; }
}