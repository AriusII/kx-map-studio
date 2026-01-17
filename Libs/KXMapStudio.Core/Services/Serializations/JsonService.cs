namespace KXMapStudio.Core.Services.Serializations;

public sealed record JsonService(IJsonDataRepository JsonDataRepository) : IJsonService
{
	public async Task<JsonModel> LoadAsync(string path, CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(path);

		return await JsonDataRepository.LoadAsync<JsonModel>(path, cancellationToken);
	}

	public async Task SaveAsync(JsonModel data, string path, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(data);
		ArgumentException.ThrowIfNullOrWhiteSpace(path);

		await JsonDataRepository.SaveAsync(data, path, cancellationToken);
	}

	public async Task<IReadOnlyList<MapModel>> LoadGuildWarsMapsAsync(string path,
		CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(path);

		var data = await JsonDataRepository.LoadAsync<IReadOnlyList<MapModel>>(path, cancellationToken);
		return data ?? [];
	}

	public async Task<ContinentFloorModel?> LoadGuildWarsContinentFloorAsync(string path,
		CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(path);

		return await JsonDataRepository.LoadAsync<ContinentFloorModel>(path, cancellationToken);
	}
}