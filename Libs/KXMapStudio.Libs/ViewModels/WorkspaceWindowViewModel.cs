namespace KXMapStudio.Libs.ViewModels;

public sealed class WorkspaceWindowViewModel(
	ILeftPanelViewModel leftPanel,
	IGridEditorViewModel gridEditor,
	IStatusBarViewModel statusBar)
	: ObservableObject, IWorkspaceWindowViewModel
{
	public ILeftPanelViewModel LeftPanel { get; } = leftPanel;
	public IGridEditorViewModel GridEditor { get; } = gridEditor;
	public IStatusBarViewModel StatusBar { get; } = statusBar;
}