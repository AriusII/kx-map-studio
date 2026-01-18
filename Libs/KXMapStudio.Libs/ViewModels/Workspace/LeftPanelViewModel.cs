namespace KXMapStudio.Libs.ViewModels.Workspace;

public sealed class LeftPanelViewModel(IFileExplorerViewModel fileExplorer) : ObservableObject, ILeftPanelViewModel
{
	public IFileExplorerViewModel FileExplorer { get; } = fileExplorer;

	public void Dispose()
	{
		FileExplorer.Dispose();
	}
}