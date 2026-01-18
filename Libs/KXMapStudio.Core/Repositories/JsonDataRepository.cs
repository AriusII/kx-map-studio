namespace KXMapStudio.Core.Repositories;

/// <summary>
///     Provides JSON persistence based on <see cref="System.Text.Json" /> and an underlying
///     <see cref="IFileStorageRepository" />.
/// </summary>
/// <param name="FileStorage">The underlying file storage abstraction.</param>
public sealed record JsonDataRepository(IFileStorageRepository FileStorage) : IJsonDataRepository
{
	/// <summary>
	///     Gets the default JSON serializer options used by Core.
	/// </summary>
	/// <remarks>
	///     These options are intentionally deterministic to keep file diffs stable and user-editable.
	/// </remarks>
	public static JsonSerializerOptions DefaultJsonOptions { get; } = new()
	{
		WriteIndented = true,
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
		Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
	};

	/// <summary>
	///     Saves the specified object as JSON to the provided path.
	/// </summary>
	/// <typeparam name="T">The type of the object to serialize.</typeparam>
	/// <param name="data">The data instance to serialize.</param>
	/// <param name="path">The target file path.</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	public async Task SaveAsync<T>(T data, string path, CancellationToken cancellationToken = default)
	{
		using var ms = new MemoryStream();
		await JsonSerializer.SerializeAsync(ms, data, DefaultJsonOptions, cancellationToken);
		ms.Position = 0;
		await FileStorage.SaveAsync(path, ms, cancellationToken);
	}

	/// <summary>
	///     Loads and deserializes JSON content from the provided path.
	/// </summary>
	/// <typeparam name="T">The target model type.</typeparam>
	/// <param name="path">The source file path.</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	/// <returns>
	///     The deserialized model instance, or <see langword="default" /> when the file is missing or the payload is invalid.
	/// </returns>
	/// <remarks>
	///     This method is intentionally tolerant: malformed JSON returns <see langword="default" /> instead of throwing.
	///     Callers that need error details should implement explicit validation at the application boundary.
	/// </remarks>
	public async Task<T?> LoadAsync<T>(string path, CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(path);

		if (!FileStorage.Exists(path))
			return default;

		await using var stream = await FileStorage.LoadAsync(path, cancellationToken);
		if (stream is null)
			return default;

		try
		{
			return await JsonSerializer.DeserializeAsync<T>(stream, DefaultJsonOptions, cancellationToken);
		}
		catch
		{
			return default;
		}
	}
}