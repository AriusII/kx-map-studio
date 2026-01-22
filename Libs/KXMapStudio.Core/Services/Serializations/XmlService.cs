namespace KXMapStudio.Core.Services.Serializations;

/// <summary>
///     Provides TacO overlay XML workflows (load/parse/save) on top of <see cref="IXmlRepository" />.
/// </summary>
/// <remarks>
///     This service orchestrates I/O through <see cref="IXmlRepository" /> and delegates XML mapping to
///     <see cref="XmlMapper" />.
/// </remarks>
/// <param name="XmlRepository">The XML repository used to read and write XML documents.</param>
internal sealed record XmlService(IXmlRepository XmlRepository) : IXmlService
{
	/// <inheritdoc />
	public async Task<MarkerModel> LoadFromFileAsync(string filePath, CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

		var document = await XmlRepository.LoadFromFileAsync(filePath, cancellationToken).ConfigureAwait(false);
		return XmlMapper.Deserialize(document);
	}

	/// <inheritdoc />
	public async Task SaveToFileAsync(MarkerModel model, string filePath, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(model);
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

		var document = XmlMapper.Serialize(model);
		await XmlRepository.SaveToFileAsync(document, filePath, cancellationToken).ConfigureAwait(false);
	}

	/// <inheritdoc />
	public async Task<MarkerModel> LoadFromStreamAsync(Stream stream, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(stream);

		var document = await XmlRepository.LoadFromArchiveStreamAsync(stream, cancellationToken).ConfigureAwait(false);
		return XmlMapper.Deserialize(document);
	}

	/// <inheritdoc />
	public async Task SaveToStreamAsync(MarkerModel model, Stream stream, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(model);
		ArgumentNullException.ThrowIfNull(stream);

		var document = XmlMapper.Serialize(model);
		await XmlRepository.SaveToArchiveStreamAsync(document, stream, cancellationToken).ConfigureAwait(false);
	}
}