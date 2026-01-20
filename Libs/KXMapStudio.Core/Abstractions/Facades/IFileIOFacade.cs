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
	// ====== TACO Marker Pack Operations ======

	/// <summary>
	///     Reads a TACO marker pack from a file (XML, ZIP, or TACO).
	/// </summary>
	/// <param name="filePath">The file path.</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	/// <returns>The parsed marker pack, or <see langword="null" /> when the file cannot be read.</returns>
	Task<MarkerModel?> ReadMarkerPackAsync(string filePath, CancellationToken cancellationToken = default);

	/// <summary>
	///     Reads a TACO marker pack from a specific entry within an archive.
	/// </summary>
	/// <param name="archivePath">The archive file path (ZIP or TACO).</param>
	/// <param name="entryFullName">The full name of the entry to read.</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	/// <returns>The parsed marker pack, or <see langword="null" /> when the entry cannot be read.</returns>
	Task<MarkerModel?> ReadMarkerFromArchiveEntryAsync(
		string archivePath,
		string entryFullName,
		CancellationToken cancellationToken = default);

	/// <summary>
	///     Saves a TACO marker pack to a file (XML format).
	/// </summary>
	/// <param name="model">The marker pack to save.</param>
	/// <param name="filePath">The destination file path.</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	Task SaveMarkerAsync(
		MarkerModel model,
		string filePath,
		CancellationToken cancellationToken = default);

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

	// ====== Archive Operations ======

	/// <summary>
	///     Lists all entry names in an archive.
	/// </summary>
	/// <param name="archivePath">The archive file path.</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	/// <returns>A read-only list of entry full names.</returns>
	Task<IReadOnlyList<string>> ListArchiveEntriesAsync(
		string archivePath,
		CancellationToken cancellationToken = default);
}