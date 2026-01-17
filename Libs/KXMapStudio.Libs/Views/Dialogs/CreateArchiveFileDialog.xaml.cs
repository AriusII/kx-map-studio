namespace KXMapStudio.Libs.Views.Dialogs;

public partial class CreateArchiveFileDialog : Window
{
	public CreateArchiveFileDialog()
	{
		InitializeComponent();
		FileNameTextBox.Focus();
	}

	public string? FileName { get; private set; }

	private void CreateButton_Click(object sender, RoutedEventArgs e)
	{
		FileName = FileNameTextBox.Text?.Trim();

		if (string.IsNullOrWhiteSpace(FileName))
		{
			MessageBox.Show("Please enter a file name.", "Validation Error", MessageBoxButton.OK,
				MessageBoxImage.Warning);
			return;
		}

		DialogResult = true;
		Close();
	}

	private void CancelButton_Click(object sender, RoutedEventArgs e)
	{
		DialogResult = false;
		Close();
	}
}