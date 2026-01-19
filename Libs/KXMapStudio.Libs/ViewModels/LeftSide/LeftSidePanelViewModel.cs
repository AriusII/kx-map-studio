namespace KXMapStudio.Libs.ViewModels.LeftSide;

public sealed class LeftSidePanelViewModel : ObservableObject, ILeftPanelViewModel, IDisposable
{
	public LeftSidePanelViewModel(IWorkshopExplorerViewModel workshopExplorer, IFilePreviewViewModel filePreview)
	{
		WorkshopExplorer = workshopExplorer;
		FilePreview = filePreview;

		if (WorkshopExplorer is WorkshopExplorerViewModel fe)
			fe.FileSelected += OnFileSelected;
	}

	public IWorkshopExplorerViewModel WorkshopExplorer { get; }
	public IFilePreviewViewModel FilePreview { get; }

	public void Dispose()
	{
		if (WorkshopExplorer is WorkshopExplorerViewModel fe)
			fe.FileSelected -= OnFileSelected;

		if (WorkshopExplorer is IDisposable d1)
			d1.Dispose();

		if (FilePreview is IDisposable d2)
			d2.Dispose();
	}

	private async void OnFileSelected(object? sender, string fullPath)
	{
		try
		{
			await FilePreview.LoadPreviewAsync(fullPath);
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Error loading file preview for '{fullPath}': {ex}");
		}
	}
}