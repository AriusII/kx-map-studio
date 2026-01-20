namespace KXMapStudio.Core.Abstractions.Services.Serializations;

/// <summary>
///     Defines TACO overlay XML workflows.
/// </summary>
public interface IXmlService
{
	/// <summary>
	///     Loads a marker pack from a file.
	/// </summary>
	/// <param name="filePath">The XML file path.</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	/// <returns>The parsed marker pack, or <see langword="null" /> when invalid.</returns>
	Task<MarkerModel> LoadFromFileAsync(string filePath, CancellationToken cancellationToken = default);

	/// <summary>
	///     Saves the specified marker pack to a file.
	/// </summary>
	/// <param name="model">The marker pack to serialize.</param>
	/// <param name="filePath">The destination XML file path.</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	Task SaveToFileAsync(MarkerModel model, string filePath, CancellationToken cancellationToken = default);

	/// <summary>
	///     Loads a marker pack from a stream.
	/// </summary>
	/// <param name="stream">The XML input stream.</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	/// <returns>The parsed marker pack, or <see langword="null" /> when invalid.</returns>
	Task<MarkerModel> LoadFromStreamAsync(Stream stream, CancellationToken cancellationToken = default);

	/// <summary>
	///     Saves the specified marker pack to a stream.
	/// </summary>
	/// <param name="model">The marker pack to serialize.</param>
	/// <param name="stream">The output stream.</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	Task SaveToStreamAsync(MarkerModel model, Stream stream, CancellationToken cancellationToken = default);
}