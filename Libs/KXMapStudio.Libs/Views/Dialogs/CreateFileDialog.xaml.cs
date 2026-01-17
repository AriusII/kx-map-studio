namespace KXMapStudio.Libs.Views.Dialogs;

public partial class CreateFileDialog : Window
{
	public CreateFileDialog()
	{
		InitializeComponent();
		FileNameTextBox.Focus();
	}

	public string? FileName { get; private set; }
	public FileType SelectedFileType { get; private set; } = FileType.Xml;

	private void CreateButton_Click(object sender, RoutedEventArgs e)
	{
		FileName = FileNameTextBox.Text?.Trim();

		if (string.IsNullOrWhiteSpace(FileName))
		{
			MessageBox.Show("Please enter a file name.", "Validation Error", MessageBoxButton.OK,
				MessageBoxImage.Warning);
			return;
		}

		SelectedFileType = XmlRadioButton.IsChecked == true
			? FileType.Xml
			: JsonKxv1RadioButton.IsChecked == true
				? FileType.JsonKxv1
				: FileType.JsonKxv2;

		DialogResult = true;
		Close();
	}

	private void CancelButton_Click(object sender, RoutedEventArgs e)
	{
		DialogResult = false;
		Close();
	}
}