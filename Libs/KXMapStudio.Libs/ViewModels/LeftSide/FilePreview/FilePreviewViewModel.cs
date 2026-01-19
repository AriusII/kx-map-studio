namespace KXMapStudio.Libs.ViewModels.LeftSide.FilePreview;

public sealed partial class FilePreviewViewModel : ObservableObject, IFilePreviewViewModel, IDisposable
{
	private readonly IFilePreviewService _previewService;
	private CancellationTokenSource? _cts;

	[ObservableProperty] private string? _openedFileName;
	[ObservableProperty] private string _previewLoadTimeText = string.Empty;
	[ObservableProperty] private string _previewStatus = string.Empty;

	public FilePreviewViewModel(IFilePreviewService previewService)
	{
		_previewService = previewService;
		PreviewTreeNodes = [];

		PreviewStatus = "No file selected.";
	}

	public void Dispose()
	{
		_cts?.Cancel();
		_cts?.Dispose();
		_cts = null;
	}

	public ObservableCollection<PreviewTreeNodeModel> PreviewTreeNodes { get; }

	public async Task LoadPreviewAsync(string fullPath)
	{
		_cts?.Cancel();
		_cts?.Dispose();
		_cts = new CancellationTokenSource();

		PreviewStatus = "Loading preview...";
		PreviewLoadTimeText = string.Empty;

		var result = await _previewService.BuildPreviewAsync(fullPath, _cts.Token);

		OpenedFileName = result.OpenedFileName;
		PreviewStatus = result.PreviewStatus;
		PreviewLoadTimeText = result.PreviewLoadTimeText;

		PreviewTreeNodes.Clear();
		foreach (var n in result.Nodes)
			PreviewTreeNodes.Add(n);
	}

	[RelayCommand]
	private static void SelectPreviewTreeNode(PreviewTreeNodeModel? node)
	{
		_ = node;
	}
}