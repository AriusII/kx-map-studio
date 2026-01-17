namespace KXMapStudio.Libs.ViewModels.Workspace;

public sealed class WorkspaceWindowViewModel : ObservableObject, IDisposable
{
	public MainMenuViewModel MainMenu { get; } = new();
	public LeftWorkspaceViewModel LeftWorkspace { get; } = new();
	public CenterWorkspaceViewModel CenterWorkspace { get; } = new();
	public RightWorkspaceViewModel RightWorkspace { get; } = new();

	public void Dispose()
	{
		LeftWorkspace.Dispose();
		CenterWorkspace.Dispose();
		RightWorkspace.Dispose();
		MainMenu.Dispose();
	}
}