namespace KXMapStudio.Libs.Abstractions;

public interface IFilePreviewService
{
	Task<FilePreviewTreeResultModel> BuildPreviewTreeAsync(string path, CancellationToken cancellationToken = default);
}