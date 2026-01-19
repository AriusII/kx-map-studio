namespace KXMapStudio.Libs.Abstractions.ViewModels;

public interface IWorkspaceWindowViewModel
{
	IMainMenuViewModel MainMenu { get; }
	ILeftPanelViewModel LeftPanel { get; }
	IGridEditorViewModel GridEditor { get; }
}