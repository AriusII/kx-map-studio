namespace KXMapStudio.Core.Abstractions.Facades;

/// <summary>
///     Provides a unified entry point (facade) for all file reading operations.
/// </summary>
/// <remarks>
///     This facade coordinates access to specialized services (XML, JSON, Archive) and provides
///     a simplified API for consuming layers. It follows the pattern: Facade → Services → Repositories.
/// </remarks>
public interface IFileIoFacade
{
	// ====== GW2 JSON Operations ======

	/// <summary>
	///     Reads Guild Wars 2 maps data from a JSON file.
	/// </summary>
	/// <param name="filePath">The JSON file path.</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	/// <returns>A read-only list of map models.</returns>
	Task<IReadOnlyList<MapModel>> ReadGw2MapsAsync(string filePath, CancellationToken cancellationToken = default);

	/// <summary>
	///     Reads Guild Wars 2 continent floor data from a JSON file.
	/// </summary>
	/// <param name="filePath">The JSON file path.</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	/// <returns>The continent floor model, or <see langword="null" /> when the file cannot be read.</returns>
	Task<ContinentFloorModel?> ReadGw2ContinentFloorAsync(
		string filePath,
		CancellationToken cancellationToken = default);

	// ====== Generic JSON Operations ======

	/// <summary>
	///     Reads a generic JSON file.
	/// </summary>
	/// <param name="filePath">The JSON file path.</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	/// <returns>The parsed JSON model.</returns>
	Task<JsonModel> ReadJsonAsync(string filePath, CancellationToken cancellationToken = default);
}