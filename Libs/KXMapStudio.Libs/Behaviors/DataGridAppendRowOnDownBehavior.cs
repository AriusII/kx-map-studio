namespace KXMapStudio.Libs.Behaviors;

/// <summary>
///     Calls an append-row command when the user presses the Down arrow while the last row is selected.
///     Keeps WPF event handling out of the ViewModel.
/// </summary>
public static class DataGridAppendRowOnDownBehavior
{
	public static readonly DependencyProperty AppendRowCommandProperty = DependencyProperty.RegisterAttached(
		"AppendRowCommand",
		typeof(ICommand),
		typeof(DataGridAppendRowOnDownBehavior),
		new PropertyMetadata(null, OnAppendRowCommandChanged));

	public static void SetAppendRowCommand(DependencyObject element, ICommand? value)
	{
		element.SetValue(AppendRowCommandProperty, value);
	}

	public static ICommand? GetAppendRowCommand(DependencyObject element)
	{
		return (ICommand?)element.GetValue(AppendRowCommandProperty);
	}

	private static void OnAppendRowCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is not DataGrid dataGrid)
			return;

		if (e.OldValue is not null)
			dataGrid.PreviewKeyDown -= DataGridOnPreviewKeyDown;

		if (e.NewValue is not null)
			dataGrid.PreviewKeyDown += DataGridOnPreviewKeyDown;
	}

	private static void DataGridOnPreviewKeyDown(object sender, KeyEventArgs e)
	{
		if (sender is not DataGrid dataGrid)
			return;

		if (e.Key != Key.Down)
			return;

		var command = GetAppendRowCommand(dataGrid);
		if (command is null)
			return;

		if (dataGrid.Items.Count <= 0)
			return;

		if (dataGrid.SelectedIndex != dataGrid.Items.Count - 1)
			return;

		dataGrid.CommitEdit(DataGridEditingUnit.Cell, true);
		dataGrid.CommitEdit(DataGridEditingUnit.Row, true);

		if (!command.CanExecute(null))
			return;

		command.Execute(null);
		e.Handled = true;
	}
}