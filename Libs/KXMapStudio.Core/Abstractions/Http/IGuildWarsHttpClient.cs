namespace KXMapStudio.Core.Abstractions.Http;

public interface IGuildWarsHttpClient
{
	public Task<ContinentFloorModel> GetContinentsAsync(CancellationToken cancellationToken = default);
	public Task<List<MapModel>> GetMapsAsync(CancellationToken cancellationToken = default);
}