namespace KXMapStudio.Application.Views.Windows;

public sealed partial class WorkspaceWindow : Window
{
	public WorkspaceWindow(WorkspaceWindowViewModel viewModel)
	{
		InitializeComponent();
		DataContext = viewModel;
	}
}