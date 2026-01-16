using KXMapStudio.Core.Models.Json.Kx.v1;

namespace KXMapStudio.Core.Services.Serializations;

public sealed record JsonService(IJsonDataRepository JsonDataRepository) : IJsonService
{
	public async Task<KxModel?> LoadKxJsonV1Async(string path, CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(path);

		return await JsonDataRepository.LoadDataAsync<KxModel>(path, cancellationToken);
	}
}