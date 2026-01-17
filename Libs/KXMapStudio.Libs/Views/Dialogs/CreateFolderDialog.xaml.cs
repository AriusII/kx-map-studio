namespace KXMapStudio.Libs.Views.Dialogs;

public partial class CreateFolderDialog : Window
{
	public CreateFolderDialog()
	{
		InitializeComponent();
		FolderNameTextBox.Focus();
	}

	public string? FolderName { get; private set; }

	private void CreateButton_Click(object sender, RoutedEventArgs e)
	{
		FolderName = FolderNameTextBox.Text?.Trim();

		if (string.IsNullOrWhiteSpace(FolderName))
		{
			MessageBox.Show("Please enter a folder name.", "Validation Error", MessageBoxButton.OK,
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