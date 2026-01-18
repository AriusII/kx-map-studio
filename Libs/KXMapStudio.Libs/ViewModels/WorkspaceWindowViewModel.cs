namespace KXMapStudio.Libs.ViewModels;

public sealed class WorkspaceWindowViewModel(ILeftPanelViewModel leftPanel)
	: ObservableObject, IWorkspaceWindowViewModel
{
	public ILeftPanelViewModel LeftPanel { get; } = leftPanel;
}