namespace KXMapStudio.Libs.ViewModels;

public sealed class WorkspaceWindowViewModel : ObservableObject, IWorkspaceWindowViewModel
{
	public WorkspaceWindowViewModel(IWorkshopExplorerViewModel workshopExplorer, ILeftPanelViewModel leftPanel)
	{
		WorkshopExplorer = workshopExplorer;
		LeftPanel = leftPanel;
	}

	public IWorkshopExplorerViewModel WorkshopExplorer { get; }
	public ILeftPanelViewModel LeftPanel { get; }
}