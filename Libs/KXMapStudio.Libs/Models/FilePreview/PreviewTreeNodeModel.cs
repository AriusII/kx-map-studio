namespace KXMapStudio.Libs.Models.FilePreview;

public sealed partial class PreviewTreeNodeModel(
	string name,
	string? fullPath = null,
	int? count = null,
	bool isLeaf = false)
	: ObservableObject
{
	[ObservableProperty] private bool _isExpanded;
	[ObservableProperty] private bool _isSelected;
	public string Name { get; } = name;
	public string? FullPath { get; } = fullPath;
	public int? Count { get; } = count;
	public bool IsLeaf { get; } = isLeaf;

	public ObservableCollection<PreviewTreeNodeModel> Children { get; } = [];
}