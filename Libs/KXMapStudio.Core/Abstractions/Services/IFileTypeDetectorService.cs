using KXMapStudio.Core.Types.Enums;

namespace KXMapStudio.Core.Abstractions.Services;

public interface IFileTypeDetectorService
{
	Task<DataType> DetectAsync(string path, CancellationToken cancellationToken = default);
}