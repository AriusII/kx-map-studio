namespace KXMapStudio.Libs.Abstractions;

public interface IFileTypeDetectorService
{
	Task<GridSourceKind> DetectAsync(string path, CancellationToken cancellationToken = default);
}