namespace KXMapStudio.Libs.Abstractions.ViewModels.LeftSide.FileExplorer;

public interface IFilePreviewViewModel
{
	string? OpenedFileName { get; }
	string PreviewStatus { get; }
	string PreviewLoadTimeText { get; }
	ObservableCollection<PreviewTreeNodeModel> PreviewTreeNodes { get; }

	IAsyncRelayCommand<PreviewTreeNodeModel?> SelectPreviewTreeNodeCommand { get; }

	Task LoadPreviewAsync(string fullPath);
}