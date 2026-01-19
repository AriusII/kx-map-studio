namespace KXMapStudio.Libs.Abstractions.Services.Dialogs;

public interface ISaveFileDialogService
{
	Task<string?> ShowSaveXmlAsync(string? suggestedFileName, CancellationToken cancellationToken = default);
	Task<string?> ShowSaveJsonAsync(string? suggestedFileName, CancellationToken cancellationToken = default);
}
