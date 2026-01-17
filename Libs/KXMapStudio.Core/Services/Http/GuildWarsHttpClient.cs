namespace KXMapStudio.Core.Services.Http;

public sealed record GuildWarsHttpClient(HttpClient HttpClient) : IGuildWarsHttpClient
{
	public async Task<ContinentFloorModel> GetContinentsAsync(CancellationToken cancellationToken = default)
	{
		var json = await HttpClient.GetStringAsync(Constants.GuildWars.ContinentsUrl, cancellationToken);
		return JsonSerializer.Deserialize<ContinentFloorModel>(json)!;
	}

	public async Task<IReadOnlyList<MapModel>> GetMapsAsync(CancellationToken cancellationToken = default)
	{
		var json = await HttpClient.GetStringAsync(Constants.GuildWars.MapsUrl, cancellationToken);
		return JsonSerializer.Deserialize<IReadOnlyList<MapModel>>(json)!;
	}
}