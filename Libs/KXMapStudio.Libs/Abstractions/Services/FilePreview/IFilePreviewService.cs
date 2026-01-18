namespace KXMapStudio.Libs.Abstractions.Services.FilePreview;

public interface IFilePreviewService
{
	Task<FilePreviewResult> BuildPreviewAsync(string fullPath, CancellationToken cancellationToken = default);
}