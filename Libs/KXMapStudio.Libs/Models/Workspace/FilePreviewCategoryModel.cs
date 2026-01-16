namespace KXMapStudio.Libs.Models.Workspace;

public sealed partial class FilePreviewCategoryModel(string name, int count) : ObservableObject
{
	public string Name { get; } = name;
	public int Count { get; } = count;

	[ObservableProperty] private bool _isSelected;
}