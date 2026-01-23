﻿namespace KXMapStudio.Libs.Views.LeftSide.WorkshopExplorer;

/// <summary>
///     Interaction logic for WorkspaceExplorerUserControl.xaml
/// </summary>
/// <remarks>
///     This code-behind contains minimal UI event forwarding logic.
///     All presentation logic resides in <see cref="IWorkshopExplorerViewModel" />.
/// </remarks>
public sealed partial class WorkspaceExplorerUserControl : UserControl
{
	public WorkspaceExplorerUserControl()
	{
		InitializeComponent();
	}

	/// <summary>
	///     Forwards TreeView selection changed event to the ViewModel command.
	/// </summary>
	private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
	{
		if (DataContext is IWorkshopExplorerViewModel vm)
			vm.SelectNodeCommand.Execute(e);
	}
}