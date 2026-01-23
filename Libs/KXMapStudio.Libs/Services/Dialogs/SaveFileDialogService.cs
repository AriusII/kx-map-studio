namespace KXMapStudio.Libs.Services.Dialogs;

public sealed class SaveFileDialogService : ISaveFileDialogService
{
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

	public Task<string?> ShowCreateJsonFileDialogAsync(string defaultFolder, CancellationToken cancellationToken = default)
	{
		_ = cancellationToken;

		var dialog = new SaveFileDialog
		{
			Title = "Create New JSON File",
			Filter = "JSON files (*.json)|*.json",
			FileName = "new_file.json",
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