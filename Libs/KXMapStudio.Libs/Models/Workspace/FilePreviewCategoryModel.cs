namespace KXMapStudio.Libs.Models.Workspace;

public sealed partial class FilePreviewCategoryModel(string name, int count) : ObservableObject
{
	[ObservableProperty] private bool _isSelected;
	public string Name { get; } = name;
	public int Count { get; } = count;
}