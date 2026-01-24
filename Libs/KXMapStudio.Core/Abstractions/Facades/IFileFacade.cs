namespace KXMapStudio.Core.Abstractions.Facades;

/// <summary>
///     Provides a unified, intent-revealing entry point (facade) for all JSON file I/O operations.
/// </summary>
/// <remarks>
///     <para>
///         This facade coordinates access to specialized services (JSON serialization, Guild Wars 2 API payloads,
///         and binary operations) and provides a simplified API for consuming layers.
///         It follows the pattern: <c>Facade → Services → Repositories → Storage</c>.
///     </para>
///     <para>
///         The facade supports:
///         <list type="bullet">
///             <item>
///                 <description>Guild Wars 2 JSON payload reading (maps and continent data)</description>
///             </item>
///             <item>
///                 <description>KX JSON model CRUD operations (create, read, write)</description>
///             </item>
///             <item>
///                 <description>Binary (byte[]) serialization for performance-critical scenarios</description>
///             </item>
///             <item>
///                 <description>Generic JSON persistence for extensibility</description>
///             </item>
///         </list>
///     </para>
/// </remarks>
public interface IFileFacade
{
	// ====== Guild Wars 2 JSON Operations (Read-Only) ======

	/// <summary>
	///     Reads Guild Wars 2 maps data from a JSON file.
	/// </summary>
	/// <param name="filePath">The JSON file path. Must not be <see langword="null" /> or whitespace.</param>
	/// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
	/// <returns>
	///     A <see cref="Task{TResult}" /> that represents the asynchronous operation.
	///     The task result contains a read-only list of map models.
	/// </returns>
	/// <exception cref="ArgumentException">Thrown when <paramref name="filePath" /> is <see langword="null" /> or whitespace.</exception>
	/// <remarks>
	///     This method is tolerant: missing or malformed files return an empty list, allowing graceful degradation.
	/// </remarks>
	Task<IReadOnlyList<MapModel>> ReadGw2MapsAsync(string filePath, CancellationToken cancellationToken = default);

	/// <summary>
	///     Reads Guild Wars 2 continent floor data from a JSON file.
	/// </summary>
	/// <param name="filePath">The JSON file path. Must not be <see langword="null" /> or whitespace.</param>
	/// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
	/// <returns>
	///     A <see cref="Task{TResult}" /> that represents the asynchronous operation.
	///     The task result contains the continent floor model, or <see langword="null" /> when the file cannot be read.
	/// </returns>
	/// <exception cref="ArgumentException">Thrown when <paramref name="filePath" /> is <see langword="null" /> or whitespace.</exception>
	Task<ContinentFloorModel?> ReadGw2ContinentFloorAsync(
		string filePath,
		CancellationToken cancellationToken = default);

	// ====== KX JSON Model Operations (Read/Write) ======

	/// <summary>
	///     Reads a KX JSON model from the specified file.
	/// </summary>
	/// <param name="filePath">The JSON file path. Must not be <see langword="null" /> or whitespace.</param>
	/// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
	/// <returns>
	///     A <see cref="Task{TResult}" /> that represents the asynchronous operation.
	///     The task result contains the parsed JSON model.
	/// </returns>
	/// <exception cref="ArgumentException">Thrown when <paramref name="filePath" /> is <see langword="null" /> or whitespace.</exception>
	/// <exception cref="InvalidDataException">Thrown when the JSON is missing or invalid.</exception>
	Task<JsonModel> ReadJsonAsync(string filePath, CancellationToken cancellationToken = default);

	/// <summary>
	///     Writes a KX JSON model to the specified file.
	/// </summary>
	/// <param name="data">The model to serialize. Must not be <see langword="null" />.</param>
	/// <param name="filePath">The output file path. Must not be <see langword="null" /> or whitespace.</param>
	/// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
	/// <returns>A <see cref="Task" /> that represents the asynchronous write operation.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="data" /> is <see langword="null" />.</exception>
	/// <exception cref="ArgumentException">Thrown when <paramref name="filePath" /> is <see langword="null" /> or whitespace.</exception>
	Task WriteJsonAsync(JsonModel data, string filePath, CancellationToken cancellationToken = default);

	/// <summary>
	///     Creates a new KX JSON model with the specified parameters and saves it to the specified file.
	/// </summary>
	/// <param name="name">The document name. Must not be <see langword="null" /> or whitespace.</param>
	/// <param name="author">The optional author name. Can be <see langword="null" />.</param>
	/// <param name="coordinates">The array of coordinate entries. Must not be <see langword="null" />.</param>
	/// <param name="outputFilePath">The destination file path. Must not be <see langword="null" /> or whitespace.</param>
	/// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
	/// <returns>
	///     A <see cref="Task{TResult}" /> that represents the asynchronous operation.
	///     The task result contains the newly created <see cref="JsonModel" /> instance.
	/// </returns>
	/// <exception cref="ArgumentException">
	///     Thrown when <paramref name="name" /> or <paramref name="outputFilePath" /> is
	///     <see langword="null" /> or whitespace.
	/// </exception>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="coordinates" /> is <see langword="null" />.</exception>
	/// <remarks>
	///     This method creates a new JSON model in memory and immediately persists it to disk.
	///     The returned model can be used for further operations without reloading from disk.
	/// </remarks>
	Task<JsonModel> CreateNewJsonAsync(
		string name,
		string? author,
		CoordinatesModel[] coordinates,
		string outputFilePath,
		CancellationToken cancellationToken = default);

	// ====== Binary Operations ======

	/// <summary>
	///     Serializes a KX JSON model to a UTF-8 encoded byte array.
	/// </summary>
	/// <param name="data">The model to serialize. Must not be <see langword="null" />.</param>
	/// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
	/// <returns>
	///     A <see cref="Task{TResult}" /> that represents the asynchronous operation.
	///     The task result contains a byte array with the UTF-8 encoded JSON representation.
	/// </returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="data" /> is <see langword="null" />.</exception>
	/// <remarks>
	///     Enables high-performance in-memory serialization without file system access.
	///     Ideal for:
	///     <list type="bullet">
	///         <item>
	///             <description>Caching scenarios</description>
	///         </item>
	///         <item>
	///             <description>Network transmission</description>
	///         </item>
	///         <item>
	///             <description>Inter-process communication</description>
	///         </item>
	///         <item>
	///             <description>Memory-mapped files</description>
	///         </item>
	///     </list>
	/// </remarks>
	Task<byte[]> SerializeJsonToBytesAsync(JsonModel data, CancellationToken cancellationToken = default);

	/// <summary>
	///     Deserializes a KX JSON model from a UTF-8 encoded byte array.
	/// </summary>
	/// <param name="jsonBytes">The UTF-8 encoded JSON byte array. Must not be <see langword="null" /> or empty.</param>
	/// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
	/// <returns>
	///     A <see cref="Task{TResult}" /> that represents the asynchronous operation.
	///     The task result contains the deserialized model, or <see langword="null" /> when the payload is invalid.
	/// </returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="jsonBytes" /> is <see langword="null" />.</exception>
	/// <exception cref="ArgumentException">Thrown when <paramref name="jsonBytes" /> is empty.</exception>
	/// <remarks>
	///     This method enables high-performance deserialization from in-memory byte arrays.
	///     Useful for network payloads, embedded resources, or cached data.
	/// </remarks>
	Task<JsonModel?> DeserializeJsonFromBytesAsync(byte[] jsonBytes, CancellationToken cancellationToken = default);
}