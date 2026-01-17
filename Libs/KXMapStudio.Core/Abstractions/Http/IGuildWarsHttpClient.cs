namespace KXMapStudio.Core.Abstractions.Http;

public interface IGuildWarsHttpClient
{
	public Task<ContinentFloorModel> GetContinentsAsync(CancellationToken cancellationToken = default);
	public Task<IReadOnlyList<MapModel>> GetMapsAsync(CancellationToken cancellationToken = default);
}