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
	Task<TacoMarkerPackModel?> LoadFromFileAsync(string filePath, CancellationToken cancellationToken = default);

	/// <summary>
	///     Saves the specified marker pack to a file.
	/// </summary>
	/// <param name="model">The marker pack to serialize.</param>
	/// <param name="filePath">The destination XML file path.</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	Task SaveToFileAsync(TacoMarkerPackModel model, string filePath, CancellationToken cancellationToken = default);

	/// <summary>
	///     Loads a marker pack from a stream.
	/// </summary>
	/// <param name="stream">The XML input stream.</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	/// <returns>The parsed marker pack, or <see langword="null" /> when invalid.</returns>
	Task<TacoMarkerPackModel?> LoadFromStreamAsync(Stream stream, CancellationToken cancellationToken = default);

	/// <summary>
	///     Saves the specified marker pack to a stream.
	/// </summary>
	/// <param name="model">The marker pack to serialize.</param>
	/// <param name="stream">The output stream.</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	Task SaveToStreamAsync(TacoMarkerPackModel model, Stream stream, CancellationToken cancellationToken = default);

	/// <summary>
	///     Parses an in-memory XML document as a TACO marker pack.
	/// </summary>
	/// <param name="document">The source XML document.</param>
	/// <returns>A marker pack instance. Returns an empty marker pack when parsing fails.</returns>
	TacoMarkerPackModel ParseTacoMarkerPack(XDocument document);
}