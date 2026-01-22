namespace KXMapStudio.Libs.ViewModels.LeftSide;

public sealed class LeftSidePanelViewModel : ObservableObject, ILeftPanelViewModel, IDisposable
{
	public LeftSidePanelViewModel(
		IWorkshopExplorerViewModel workshopExplorer,
		IGridEditorViewModel gridEditor)
	{
		WorkshopExplorer = workshopExplorer;
		GridEditor = gridEditor;

		if (WorkshopExplorer is WorkshopExplorerViewModel fe)
			fe.FileSelected += OnFileSelected;
	}

	public IWorkshopExplorerViewModel WorkshopExplorer { get; }
	public IGridEditorViewModel GridEditor { get; }

	public void Dispose()
	{
		if (WorkshopExplorer is WorkshopExplorerViewModel fe)
			fe.FileSelected -= OnFileSelected;

		if (WorkshopExplorer is IDisposable d1)
			d1.Dispose();

		if (GridEditor is IDisposable d3)
			d3.Dispose();
	}

	private async void OnFileSelected(object? sender, EditorDocumentReference doc)
	{
		try
		{
			await GridEditor.LoadAsync(doc);
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Error loading selection '{doc}': {ex}");
		}
	}
}