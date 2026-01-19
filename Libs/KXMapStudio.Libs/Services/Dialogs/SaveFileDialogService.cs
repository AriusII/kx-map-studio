using Microsoft.Win32;

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
}
