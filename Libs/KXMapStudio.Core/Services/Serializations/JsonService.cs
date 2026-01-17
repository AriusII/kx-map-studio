namespace KXMapStudio.Core.Services.Serializations;

public sealed record JsonService(IJsonDataRepository JsonDataRepository) : IJsonService
{
	public async Task<DataType> DetectJsonTypeAsync(string path, CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(path);

		if (!File.Exists(path)) return DataType.Unknown;

		await using var stream = File.OpenRead(path);
		using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

		var root = doc.RootElement;

		if (root.TryGetProperty("Name", out _) && root.TryGetProperty("Coordinates", out _))
			return DataType.KxV1Json;

		if (root.TryGetProperty("regions", out _))
			return DataType.GuildWarsJson;

		if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
		{
			var first = root[0];
			if (first.TryGetProperty("continent_id", out _))
				return DataType.GuildWarsJson;
		}

		if (root.TryGetProperty("version", out _) || root.TryGetProperty("markers", out _))
			return DataType.KxV2Json;

		return DataType.Unknown;
	}

	public async Task<KxModel?> LoadKxJsonV1Async(string path, CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(path);

		return await JsonDataRepository.LoadDataAsync<KxModel>(path, cancellationToken);
	}

	public async Task SaveKxJsonV1Async(KxModel data, string path, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(data);
		ArgumentException.ThrowIfNullOrWhiteSpace(path);

		await JsonDataRepository.SaveDataAsync(data, path, cancellationToken);
	}

	public async Task<IReadOnlyList<MapModel>> LoadGuildWarsMapsAsync(string path,
		CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(path);

		var data = await JsonDataRepository.LoadDataAsync<IReadOnlyList<MapModel>>(path, cancellationToken);
		return data ?? [];
	}

	public async Task SaveGuildWarsMapsAsync(IEnumerable<MapModel> maps, string path,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(maps);
		ArgumentException.ThrowIfNullOrWhiteSpace(path);

		await JsonDataRepository.SaveDataAsync(maps.ToList(), path, cancellationToken);
	}

	public async Task<ContinentFloorModel?> LoadGuildWarsContinentFloorAsync(string path,
		CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(path);

		return await JsonDataRepository.LoadDataAsync<ContinentFloorModel>(path, cancellationToken);
	}

	public async Task SaveGuildWarsContinentFloorAsync(ContinentFloorModel model, string path,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(model);
		ArgumentException.ThrowIfNullOrWhiteSpace(path);

		await JsonDataRepository.SaveDataAsync(model, path, cancellationToken);
	}
}