namespace KXMapStudio.Core.Abstractions.Services;

/// <summary>
///     Defines high-level file workflows used by KXMapStudio.
/// </summary>
/// <remarks>
///     This API is designed as a single orchestration entry point for upper layers.
/// </remarks>
public interface IFileReaderService
{
	/// <summary>
	///     Reads a TACO marker pack from the given file.
	/// </summary>
	/// <param name="filePath">The source file path.</param>
	/// <param name="fileType">The file type used to choose the proper loader.</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	/// <returns>The loaded marker pack, or <see langword="null" /> when invalid or missing.</returns>
	Task<TacoMarkerPackModel?> ReadTacoAsync(string filePath, FileType fileType,
		CancellationToken cancellationToken = default);

	/// <summary>
	///     Reads a TACO marker pack from a specific entry inside an archive.
	/// </summary>
	/// <param name="archiveFilePath">The archive file path.</param>
	/// <param name="fileType">The archive type (ZIP/TACO).</param>
	/// <param name="entryFullName">The archive entry full name (ZIP path with '/').</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	/// <returns>The loaded marker pack, or <see langword="null" /> when invalid or missing.</returns>
	Task<TacoMarkerPackModel?> ReadTacoAsync(
		string archiveFilePath,
		FileType fileType,
		string entryFullName,
		CancellationToken cancellationToken = default);

	/// <summary>
	///     Reads the cached GW2 maps JSON payload.
	/// </summary>
	/// <param name="filePath">The JSON file path.</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	/// <returns>A read-only list of maps.</returns>
	Task<IReadOnlyList<MapModel>> ReadMapsJsonAsync(string filePath, CancellationToken cancellationToken = default);

	/// <summary>
	///     Reads the cached GW2 continent floor JSON payload.
	/// </summary>
	/// <param name="filePath">The JSON file path.</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	/// <returns>The parsed model, or <see langword="null" /> if missing or invalid.</returns>
	Task<ContinentFloorModel?> ReadContinentsJsonAsync(string filePath, CancellationToken cancellationToken = default);

	/// <summary>
	///     Reads a KX JSON file into a <see cref="JsonModel" />.
	/// </summary>
	/// <param name="filePath">The JSON file path.</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	/// <returns>The parsed <see cref="JsonModel" /> instance.</returns>
	Task<JsonModel> ReadJsonAsync(string filePath, CancellationToken cancellationToken = default);
}