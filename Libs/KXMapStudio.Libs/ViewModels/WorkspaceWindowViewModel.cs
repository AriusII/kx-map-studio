namespace KXMapStudio.Libs.ViewModels;

using KXMapStudio.Libs.Abstractions.ViewModels.TopSide.MainMenu;

public sealed class WorkspaceWindowViewModel : ObservableObject, IWorkspaceWindowViewModel
{
	public WorkspaceWindowViewModel(
		IMainMenuViewModel mainMenu,
		ILeftPanelViewModel leftPanel,
		IGridEditorViewModel gridEditor)
	{
		MainMenu = mainMenu;
		LeftPanel = leftPanel;
		GridEditor = gridEditor;
	}

	public IMainMenuViewModel MainMenu { get; }
	public ILeftPanelViewModel LeftPanel { get; }
	public IGridEditorViewModel GridEditor { get; }
}