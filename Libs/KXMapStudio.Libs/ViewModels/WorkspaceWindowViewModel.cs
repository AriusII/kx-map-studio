using KXMapStudio.Libs.ViewModels.Workspace;

namespace KXMapStudio.Libs.ViewModels;

public sealed class WorkspaceWindowViewModel : ObservableObject, IDisposable
{
	public LeftPanelViewModel LeftPanel { get; } = new();

	public void Dispose()
	{
	}
}