using KXMapStudio.Core.Models.Json.Kx.v1;

namespace KXMapStudio.Core.Abstractions.Services;

public interface IJsonService
{
	Task<KxModel?> LoadKxJsonV1Async(string path, CancellationToken cancellationToken = default);
}