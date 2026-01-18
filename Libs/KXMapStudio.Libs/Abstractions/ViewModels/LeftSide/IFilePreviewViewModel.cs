namespace KXMapStudio.Libs.Abstractions.ViewModels.LeftSide;

public interface IFilePreviewViewModel
{
	string? OpenedFileName { get; }
	string PreviewStatus { get; }
	string PreviewLoadTimeText { get; }
	ObservableCollection<PreviewTreeNodeModel> PreviewTreeNodes { get; }
	Task LoadPreviewAsync(string fullPath);
}