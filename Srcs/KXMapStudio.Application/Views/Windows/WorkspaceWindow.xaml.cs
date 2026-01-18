namespace KXMapStudio.Application.Views.Windows;

public sealed partial class WorkspaceWindow : Window
{
	public WorkspaceWindow(IWorkspaceWindowViewModel viewModel)
	{
		InitializeComponent();
		DataContext = viewModel;
	}
}