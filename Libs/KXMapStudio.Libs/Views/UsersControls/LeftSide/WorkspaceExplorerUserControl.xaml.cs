namespace KXMapStudio.Libs.Views.UsersControls.LeftSide;

public sealed partial class WorkspaceExplorerUserControl : UserControl
{
	public WorkspaceExplorerUserControl()
	{
		InitializeComponent();
	}

	public WorkspaceExplorerUserControl(FileExplorerViewModel viewModel) : this()
	{
		DataContext = viewModel;
	}
}