namespace KXMapStudio.Libs.ViewModels.LeftSide.FilePreview;

public sealed partial class FilePreviewViewModel : ObservableObject, IFilePreviewViewModel, IDisposable
{
	private readonly IFilePreviewService _previewService;
	private CancellationTokenSource? _cts;

	[ObservableProperty] private string? _openedFileName;
	[ObservableProperty] private string _previewLoadTimeText = string.Empty;
	[ObservableProperty] private string _previewStatus = string.Empty;
	private PreviewTreeNodeModel? _selectedArchiveXml;

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

	public event EventHandler<EditorDocumentReference>? DocumentSelected;

	[RelayCommand]
	private async Task SelectPreviewTreeNodeAsync(PreviewTreeNodeModel? node)
	{
		if (node is null)
			return;

		// Selecting an archive XML entry expands it (multi-level exploration) instead of directly loading the grid.
		if (node.IsArchiveEntryLeaf && string.Equals(Path.GetExtension(node.ArchiveEntryFullName), ".xml",
			    StringComparison.OrdinalIgnoreCase))
		{
			// Avoid re-expanding over and over.
			if (node.Children.Count == 0)
			{
				var ct = _cts?.Token ?? CancellationToken.None;
				var children = await _previewService.ExpandArchiveXmlAsync(node, ct);

				node.Children.Clear();
				foreach (var c in children)
					node.Children.Add(c);
			}

			node.IsExpanded = true;
			_selectedArchiveXml = node;
			return;
		}

		// Selecting an XML child node drives the grid editor.
		if (node.IsXmlNode)
		{
			if (_selectedArchiveXml?.IsArchiveEntryLeaf == true)
			{
				var doc = EditorDocumentReference.FromArchiveEntry(
					_selectedArchiveXml.ArchivePath!,
					_selectedArchiveXml.ArchiveEntryFullName!);

				DocumentSelected?.Invoke(this, doc);
			}

			return;
		}

		// Back-compat: selecting an XML file entry (outside of the new XML node flow) can still open it.
		if (node.IsArchiveEntryLeaf)
		{
			var doc = EditorDocumentReference.FromArchiveEntry(node.ArchivePath!, node.ArchiveEntryFullName!);

			// Only XML entries should drive the grid editor for now.
			if (string.Equals(doc.Extension, ".xml", StringComparison.OrdinalIgnoreCase))
				DocumentSelected?.Invoke(this, doc);
		}
	}
}