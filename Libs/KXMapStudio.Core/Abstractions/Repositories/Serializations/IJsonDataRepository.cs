namespace KXMapStudio.Core.Abstractions.Repositories.Serializations;

/// <summary>
///     Defines a persistence boundary for JSON files.
/// </summary>
public interface IJsonDataRepository
{
	/// <summary>
	///     Serializes and saves the specified data to disk.
	/// </summary>
	/// <typeparam name="T">The type of data to write.</typeparam>
	/// <param name="data">The data instance to serialize.</param>
	/// <param name="outputFile">The destination file path.</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	Task SaveAsync<T>(T data, string outputFile, CancellationToken cancellationToken = default);

	/// <summary>
	///     Loads and deserializes JSON content from disk.
	/// </summary>
	/// <typeparam name="T">The target model type.</typeparam>
	/// <param name="path">The file path.</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	/// <returns>The deserialized instance, or <see langword="default" /> when missing or invalid.</returns>
	Task<T?> LoadAsync<T>(string path, CancellationToken cancellationToken = default);
}