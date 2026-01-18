namespace KXMapStudio.Libs.ViewModels;

public sealed class WorkspaceWindowViewModel : ObservableObject, IWorkspaceWindowViewModel
{
	public WorkspaceWindowViewModel(IFileExplorerViewModel fileExplorer, ILeftPanelViewModel leftPanel)
	{
		FileExplorer = fileExplorer;
		LeftPanel = leftPanel;
	}

	public IFileExplorerViewModel FileExplorer { get; }
	public ILeftPanelViewModel LeftPanel { get; }
}