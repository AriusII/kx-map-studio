namespace KXMapStudio.Libs.Services.Dialogs;

public sealed class SaveFileDialogService : ISaveFileDialogService
{
	public Task<string?> ShowSaveXmlAsync(string? suggestedFileName, CancellationToken cancellationToken = default)
	{
		_ = cancellationToken;

		var dialog = new SaveFileDialog
		{
			Title = "Save XML As",
			Filter = "XML files (*.xml)|*.xml",
			FileName = string.IsNullOrWhiteSpace(suggestedFileName) ? "document.xml" : suggestedFileName
		};

		return Task.FromResult(dialog.ShowDialog() == true ? dialog.FileName : null);
	}

	public Task<string?> ShowSaveJsonAsync(string? suggestedFileName, CancellationToken cancellationToken = default)
	{
		_ = cancellationToken;

		var dialog = new SaveFileDialog
		{
			Title = "Save JSON As",
			Filter = "JSON files (*.json)|*.json",
			FileName = string.IsNullOrWhiteSpace(suggestedFileName) ? "document.json" : suggestedFileName
		};

		return Task.FromResult(dialog.ShowDialog() == true ? dialog.FileName : null);
	}

	public Task<string?> ShowCreateFileDialogAsync(string defaultFolder, string fileType,
		CancellationToken cancellationToken = default)
	{
		_ = cancellationToken;

		var isXml = string.Equals(fileType, "xml", StringComparison.OrdinalIgnoreCase);
		var extension = isXml ? ".xml" : ".json";
		var dialog = new SaveFileDialog
		{
			Title = $"Create New {fileType.ToUpperInvariant()} File",
			Filter = isXml ? "XML files (*.xml)|*.xml" : "JSON files (*.json)|*.json",
			FileName = $"new_file{extension}",
			InitialDirectory = Directory.Exists(defaultFolder) ? defaultFolder : null
		};

		return Task.FromResult(dialog.ShowDialog() == true ? dialog.FileName : null);
	}

	public Task<bool> ShowDeleteFileConfirmationAsync(string fileName)
	{
		var result = MessageBox.Show(
			$"Are you sure you want to delete '{fileName}'?\n\nThis action cannot be undone.",
			"Confirm Delete",
			MessageBoxButton.YesNo,
			MessageBoxImage.Warning,
			MessageBoxResult.No);

		return Task.FromResult(result == MessageBoxResult.Yes);
	}
}