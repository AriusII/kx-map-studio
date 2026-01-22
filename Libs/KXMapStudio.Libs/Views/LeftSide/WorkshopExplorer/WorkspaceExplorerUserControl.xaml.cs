namespace KXMapStudio.Libs.Views.LeftSide.WorkshopExplorer;

public sealed partial class WorkspaceExplorerUserControl : UserControl
{
	public WorkspaceExplorerUserControl()
	{
		InitializeComponent();
	}

	private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
	{
		if (DataContext is IWorkshopExplorerViewModel vm)
			vm.SelectNodeCommand.Execute(e);
	}
}