namespace KXMapStudio.Core.Abstractions.Services.Serializations;

/// <summary>
///     Defines JSON-oriented workflows used by KXMapStudio.
/// </summary>
public interface IJsonService
{
	/// <summary>
	///     Loads a KX JSON model from the specified file.
	/// </summary>
	/// <param name="path">The JSON file path.</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	/// <returns>The parsed model.</returns>
	Task<JsonModel> LoadAsync(string path, CancellationToken cancellationToken = default);

	/// <summary>
	///     Saves a KX JSON model to the specified file.
	/// </summary>
	/// <param name="data">The model to serialize.</param>
	/// <param name="path">The output file path.</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	Task SaveAsync(JsonModel data, string path, CancellationToken cancellationToken = default);

	/// <summary>
	///     Loads the GW2 maps payload from the specified file.
	/// </summary>
	/// <param name="path">The JSON file path.</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	/// <returns>A read-only list of maps.</returns>
	Task<IReadOnlyList<MapModel>> LoadGuildWarsMapsAsync(string path, CancellationToken cancellationToken = default);

	/// <summary>
	///     Loads the GW2 continent floor payload from the specified file.
	/// </summary>
	/// <param name="path">The JSON file path.</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	/// <returns>The parsed payload, or <see langword="null" /> when missing or invalid.</returns>
	Task<ContinentFloorModel?> LoadGuildWarsContinentFloorAsync(string path,
		CancellationToken cancellationToken = default);
}