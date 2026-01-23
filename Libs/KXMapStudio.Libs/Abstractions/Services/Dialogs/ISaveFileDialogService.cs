namespace KXMapStudio.Libs.Abstractions.Services.Dialogs;

public interface ISaveFileDialogService
{
	Task<string?> ShowSaveJsonAsync(string? suggestedFileName, CancellationToken cancellationToken = default);

	Task<string?> ShowCreateJsonFileDialogAsync(string defaultFolder, CancellationToken cancellationToken = default);

	Task<bool> ShowDeleteFileConfirmationAsync(string fileName);
}