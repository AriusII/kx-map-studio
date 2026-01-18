namespace KXMapStudio.Core.Abstractions.Repositories.Serializations;

/// <summary>
///     Defines a persistence boundary for XML documents used by Core workflows.
/// </summary>
public interface IXmlDataRepository
{
	/// <summary>
	///     Loads an XML document from disk.
	/// </summary>
	/// <param name="filePath">The source file path.</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	/// <returns>The loaded document, or <see langword="null" /> when missing or invalid.</returns>
	Task<XDocument?> LoadFromFileAsync(string filePath, CancellationToken cancellationToken = default);

	/// <summary>
	///     Saves an XML document to disk.
	/// </summary>
	/// <param name="document">The document to persist.</param>
	/// <param name="path">The destination file path.</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	Task SaveToFileAsync(XDocument document, string path, CancellationToken cancellationToken = default);

	/// <summary>
	///     Loads an XML document from a stream.
	/// </summary>
	/// <param name="stream">The source stream.</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	/// <returns>The loaded document, or <see langword="null" /> when invalid.</returns>
	Task<XDocument?> LoadFromStreamAsync(Stream stream, CancellationToken cancellationToken = default);

	/// <summary>
	///     Saves an XML document to a stream.
	/// </summary>
	/// <param name="document">The document to write.</param>
	/// <param name="stream">The destination stream.</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	Task SaveToStreamAsync(XDocument document, Stream stream, CancellationToken cancellationToken = default);
}