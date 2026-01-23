namespace KXMapStudio.Libs.Views.RightSide.GridEditor;

public sealed partial class GridEditorUserControl : UserControl
{
	public GridEditorUserControl()
	{
		InitializeComponent();
		DataContextChanged += OnDataContextChanged;
	}

	private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
	{
		// Unsubscribe from old ViewModel
		if (e.OldValue is IGridEditorViewModel oldVm) oldVm.RowAdded -= OnRowAdded;

		// Subscribe to new ViewModel
		if (e.NewValue is IGridEditorViewModel newVm) newVm.RowAdded += OnRowAdded;
	}

	private void OnRowAdded(object? sender, GridEditorRowViewModel addedRow)
	{
		// Use Dispatcher to ensure we're on UI thread and DataGrid is updated
		Dispatcher.BeginInvoke(new Action(() =>
		{
			// Select the newly added row
			MarkersDataGrid.SelectedItem = addedRow;

			// Scroll to make it visible
			MarkersDataGrid.ScrollIntoView(addedRow);

			// Set focus to the DataGrid and begin edit on Name column
			MarkersDataGrid.Focus();

			// Wait for the visual tree to update, then focus the Name cell
			MarkersDataGrid.UpdateLayout();

			var row = MarkersDataGrid.ItemContainerGenerator.ContainerFromItem(addedRow) as DataGridRow;
			if (row != null) row.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
		}), DispatcherPriority.Background);
	}

	private void MarkersDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
	{
		if (e.Key == Key.Delete && DataContext is IGridEditorViewModel vm)
		{
			var selectedRows = MarkersDataGrid.SelectedItems.Cast<GridEditorRowViewModel>().ToList();
			foreach (var row in selectedRows) vm.DeleteRowCommand.Execute(row);
			e.Handled = true;
		}
	}
}