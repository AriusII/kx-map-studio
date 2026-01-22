namespace KXMapStudio.Libs.ViewModels;

public sealed class WorkspaceWindowViewModel(
	IMainMenuViewModel mainMenu,
	ILeftPanelViewModel leftPanel,
	IGridEditorViewModel gridEditor,
	IStatusBarViewModel statusBar)
	: ObservableObject, IWorkspaceWindowViewModel
{
	public IMainMenuViewModel MainMenu { get; } = mainMenu;
	public ILeftPanelViewModel LeftPanel { get; } = leftPanel;
	public IGridEditorViewModel GridEditor { get; } = gridEditor;
	public IStatusBarViewModel StatusBar { get; } = statusBar;
}