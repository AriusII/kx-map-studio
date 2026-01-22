namespace KXMapStudio.Libs.Abstractions.ViewModels;

public interface IWorkspaceWindowViewModel
{
	ILeftPanelViewModel LeftPanel { get; }
	IGridEditorViewModel GridEditor { get; }
	IStatusBarViewModel StatusBar { get; }
}