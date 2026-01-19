namespace KXMapStudio.Libs.ViewModels.LeftSide;

public sealed class LeftSidePanelViewModel : ObservableObject, ILeftPanelViewModel, IDisposable
{
	public LeftSidePanelViewModel(
		IWorkshopExplorerViewModel workshopExplorer,
		IFilePreviewViewModel filePreview,
		IGridEditorViewModel gridEditor)
	{
		WorkshopExplorer = workshopExplorer;
		FilePreview = filePreview;
		GridEditor = gridEditor;

		if (WorkshopExplorer is WorkshopExplorerViewModel fe)
			fe.FileSelected += OnFileSelected;

		if (FilePreview is FilePreviewViewModel fp)
			fp.DocumentSelected += OnDocumentSelected;
	}

	public IWorkshopExplorerViewModel WorkshopExplorer { get; }
	public IFilePreviewViewModel FilePreview { get; }
	public IGridEditorViewModel GridEditor { get; }

	public void Dispose()
	{
		if (WorkshopExplorer is WorkshopExplorerViewModel fe)
			fe.FileSelected -= OnFileSelected;

		if (FilePreview is FilePreviewViewModel fp)
			fp.DocumentSelected -= OnDocumentSelected;

		if (WorkshopExplorer is IDisposable d1)
			d1.Dispose();

		if (FilePreview is IDisposable d2)
			d2.Dispose();

		if (GridEditor is IDisposable d3)
			d3.Dispose();
	}

	private async void OnFileSelected(object? sender, EditorDocumentReference doc)
	{
		try
		{
			if (doc.FilePath is { Length: > 0 } fullPath)
				await FilePreview.LoadPreviewAsync(fullPath);

			await GridEditor.LoadAsync(doc);
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Error loading selection '{doc}': {ex}");
		}
	}

	private async void OnDocumentSelected(object? sender, EditorDocumentReference doc)
	{
		try
		{
			await GridEditor.LoadAsync(doc);
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Error loading archive entry '{doc}': {ex}");
		}
	}
}