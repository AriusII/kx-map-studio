namespace KXMapStudio.Core.Services.Serializations;

/// <summary>
///     Provides JSON-oriented domain workflows on top of <see cref="IJsonDataRepository" />.
/// </summary>
/// <param name="JsonDataRepository">The JSON persistence repository.</param>
public sealed record JsonService(IJsonDataRepository JsonDataRepository) : IJsonService
{
	/// <summary>
	///     Loads the core KX JSON model from disk.
	/// </summary>
	/// <param name="path">The JSON file path.</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	/// <returns>The parsed <see cref="JsonModel" /> instance.</returns>
	/// <exception cref="ArgumentException">Thrown when <paramref name="path" /> is null or whitespace.</exception>
	/// <exception cref="InvalidDataException">Thrown when JSON is missing or invalid.</exception>
	public async Task<JsonModel> LoadAsync(string path, CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(path);

		return await JsonDataRepository.LoadAsync<JsonModel>(path, cancellationToken)
		       ?? throw new InvalidDataException($"Unable to load JSON model from '{path}'.");
	}

	/// <summary>
	///     Saves a core KX JSON model to disk.
	/// </summary>
	/// <param name="data">The model to serialize.</param>
	/// <param name="path">The JSON file path.</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	public async Task SaveAsync(JsonModel data, string path, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(data);
		ArgumentException.ThrowIfNullOrWhiteSpace(path);

		await JsonDataRepository.SaveAsync(data, path, cancellationToken);
	}

	/// <summary>
	///     Loads the cached GW2 maps payload from disk.
	/// </summary>
	/// <param name="path">The JSON file path.</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	/// <returns>A read-only list of maps; returns an empty list when missing or invalid.</returns>
	public async Task<IReadOnlyList<MapModel>> LoadGuildWarsMapsAsync(string path,
		CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(path);

		var data = await JsonDataRepository.LoadAsync<IReadOnlyList<MapModel>>(path, cancellationToken);
		return data ?? [];
	}

	/// <summary>
	///     Loads the cached GW2 continent floor payload from disk.
	/// </summary>
	/// <param name="path">The JSON file path.</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	/// <returns>The parsed model, or <see langword="null" /> when missing or invalid.</returns>
	public async Task<ContinentFloorModel?> LoadGuildWarsContinentFloorAsync(string path,
		CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(path);

		return await JsonDataRepository.LoadAsync<ContinentFloorModel>(path, cancellationToken);
	}
}