namespace KXMapStudio.Libs.ViewModels.Workspace;

public sealed class WorkspaceWindowViewModel : ObservableObject, IDisposable
{
	public MainMenuViewModel MainMenu { get; } = new();
	public LeftWorkspaceViewModel LeftWorkspace { get; } = new();
	public GridEditorViewModel GridEditor { get; } = new();
	public RightWorkspaceViewModel RightWorkspace { get; } = new();

	public void Dispose()
	{
		LeftWorkspace.Dispose();
		GridEditor.Dispose();
		RightWorkspace.Dispose();
		MainMenu.Dispose();
	}
}