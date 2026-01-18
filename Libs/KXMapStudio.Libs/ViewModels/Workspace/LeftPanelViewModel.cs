namespace KXMapStudio.Libs.ViewModels.Workspace;

public sealed class LeftPanelViewModel : ObservableObject, ILeftPanelViewModel, IDisposable
{
	public LeftPanelViewModel(IFileExplorerViewModel fileExplorer, IFilePreviewViewModel filePreview)
	{
		FileExplorer = fileExplorer;
		FilePreview = filePreview;

		if (FileExplorer is FileExplorerViewModel fe)
			fe.FileSelected += OnFileSelected;
	}

	public IFileExplorerViewModel FileExplorer { get; }
	public IFilePreviewViewModel FilePreview { get; }

	public void Dispose()
	{
		if (FileExplorer is FileExplorerViewModel fe)
			fe.FileSelected -= OnFileSelected;

		if (FileExplorer is IDisposable d1)
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
		catch (System.Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Error loading file preview for '{fullPath}': {ex}");
		}
	}
}