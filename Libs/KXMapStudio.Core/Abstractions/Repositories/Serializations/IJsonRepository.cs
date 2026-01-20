namespace KXMapStudio.Core.Abstractions.Repositories.Serializations;

/// <summary>
///     Defines a generic repository for reading and writing strongly-typed JSON models.
/// </summary>
/// <typeparam name="TModel">The target model type for deserialization/serialization.</typeparam>
public interface IJsonRepository<TModel>
	where TModel : class
{
	/// <summary>
	///     Reads and deserializes a JSON document from a stream.
	/// </summary>
	/// <param name="jsonStream">The source stream.</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	/// <returns>The deserialized model instance, or <see langword="null" /> when invalid.</returns>
	Task<TModel?> ReadAsync(Stream jsonStream, CancellationToken cancellationToken = default);

	/// <summary>
	///     Serializes and writes a model instance to a JSON stream.
	/// </summary>
	/// <param name="targetStream">The destination stream.</param>
	/// <param name="model">The model instance to serialize.</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	Task WriteAsync(Stream targetStream, TModel model, CancellationToken cancellationToken = default);
}