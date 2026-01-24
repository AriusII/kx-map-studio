namespace KXMapStudio.Core.Facades;

/// <summary>
///     Provides a unified, high-level entry point (facade) for all JSON file I/O operations.
/// </summary>
/// <param name="jsonService">The JSON service providing business-level JSON workflows.</param>
/// <param name="logger">The logger instance for operation tracing.</param>
/// <remarks>
///     <para>
///         This facade coordinates access to specialized services and provides a simplified,
///         intent-revealing API for consuming layers. It follows the pattern:
///         <c>Facade → Services → Repositories → Storage</c>.
///     </para>
///     <para>
///         The facade supports:
///         <list type="bullet">
///             <item>
///                 <description>KX JSON model CRUD operations</description>
///             </item>
///             <item>
///                 <description>Guild Wars 2 API payload caching</description>
///             </item>
///             <item>
///                 <description>Binary (byte[]) operations for performance-critical scenarios</description>
///             </item>
///             <item>
///                 <description>Generic JSON persistence for extensibility</description>
///             </item>
///         </list>
///     </para>
/// </remarks>
public sealed class FileFacade(
	IJsonService jsonService,
	ILogger<FileFacade> logger)
	: IFileFacade
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
	///     This method is tolerant: missing or malformed files return an empty list.
	/// </remarks>
	public Task<IReadOnlyList<MapModel>> ReadGw2MapsAsync(
		string filePath,
		CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

		logger.LogDebug("Facade: Reading GW2 maps from: {Path}", filePath);

		return jsonService.LoadGuildWarsMapsAsync(filePath, cancellationToken);
	}

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
	public Task<ContinentFloorModel?> ReadGw2ContinentFloorAsync(
		string filePath,
		CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

		logger.LogDebug("Facade: Reading GW2 continent floor from: {Path}", filePath);

		return jsonService.LoadGuildWarsContinentFloorAsync(filePath, cancellationToken);
	}

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
	public Task<JsonModel> ReadJsonAsync(string filePath, CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

		logger.LogDebug("Facade: Reading KX JSON model from: {Path}", filePath);

		return jsonService.LoadAsync(filePath, cancellationToken);
	}

	/// <summary>
	///     Writes a KX JSON model to the specified file.
	/// </summary>
	/// <param name="data">The model to serialize. Must not be <see langword="null" />.</param>
	/// <param name="filePath">The output file path. Must not be <see langword="null" /> or whitespace.</param>
	/// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
	/// <returns>A <see cref="Task" /> that represents the asynchronous write operation.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="data" /> is <see langword="null" />.</exception>
	/// <exception cref="ArgumentException">Thrown when <paramref name="filePath" /> is <see langword="null" /> or whitespace.</exception>
	public Task WriteJsonAsync(JsonModel data, string filePath, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(data);
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

		logger.LogInformation("Facade: Writing KX JSON model to: {Path}", filePath);

		return jsonService.SaveAsync(data, filePath, cancellationToken);
	}

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
	public Task<JsonModel> CreateNewJsonAsync(
		string name,
		string? author,
		CoordinatesModel[] coordinates,
		string outputFilePath,
		CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(name);
		ArgumentNullException.ThrowIfNull(coordinates);
		ArgumentException.ThrowIfNullOrWhiteSpace(outputFilePath);

		logger.LogInformation(
			"Facade: Creating new KX JSON model '{Name}' with {Count} coordinates at: {Path}",
			name, coordinates.Length, outputFilePath);

		return jsonService.CreateNewAsync(name, author, coordinates, outputFilePath, cancellationToken);
	}

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
	///     Ideal for caching, network transmission, or inter-process communication scenarios.
	/// </remarks>
	public Task<byte[]> SerializeJsonToBytesAsync(JsonModel data, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(data);

		logger.LogDebug("Facade: Serializing KX JSON model '{Name}' to byte array", data.Name);

		return jsonService.SerializeModelToBytesAsync(data, cancellationToken);
	}

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
	public Task<JsonModel?> DeserializeJsonFromBytesAsync(
		byte[] jsonBytes,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(jsonBytes);

		if (jsonBytes.Length == 0)
		{
			logger.LogWarning("Facade: Attempted to deserialize empty byte array");
			throw new ArgumentException("Byte array cannot be empty", nameof(jsonBytes));
		}

		logger.LogDebug("Facade: Deserializing KX JSON model from {ByteCount} bytes", jsonBytes.Length);

		return jsonService.DeserializeModelFromBytesAsync(jsonBytes, cancellationToken);
	}
}