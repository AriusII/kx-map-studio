namespace KXMapStudio.Libs.Abstractions;

public interface IFilePreviewService
{
	Task<FilePreviewResultModel> BuildPreviewAsync(string path, CancellationToken cancellationToken = default);
}