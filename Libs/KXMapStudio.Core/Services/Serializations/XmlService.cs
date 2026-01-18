namespace KXMapStudio.Core.Services.Serializations;

/// <summary>
///     Provides TACO overlay XML workflows (load/parse/save) on top of <see cref="IXmlDataRepository" />.
/// </summary>
/// <remarks>
///     This service is intentionally tolerant while parsing: malformed XML payloads return <see langword="null" />.
/// </remarks>
/// <param name="XmlDataRepository">The XML repository used to read and write XML documents.</param>
public sealed record XmlService(IXmlDataRepository XmlDataRepository) : IXmlService
{
	private static readonly XmlSerializer Serializer = new(typeof(TacoOverlayDataDto));

	/// <inheritdoc />
	public async Task<TacoMarkerPackModel?> LoadFromFileAsync(string filePath,
		CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

		var doc = await XmlDataRepository.LoadFromFileAsync(filePath, cancellationToken);
		return doc is null ? null : DeserializeXmlDocument(doc);
	}

	/// <inheritdoc />
	public async Task SaveToFileAsync(TacoMarkerPackModel model, string filePath,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(model);
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

		var xmlContent = SerializeXmlModel(model);
		await XmlDataRepository.SaveToFileAsync(xmlContent, filePath, cancellationToken);
	}

	/// <inheritdoc />
	public async Task<TacoMarkerPackModel?> LoadFromStreamAsync(Stream stream,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(stream);

		var doc = await XmlDataRepository.LoadFromStreamAsync(stream, cancellationToken);
		return doc is null ? null : DeserializeXmlDocument(doc);
	}

	/// <inheritdoc />
	public async Task SaveToStreamAsync(TacoMarkerPackModel model, Stream stream,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(model);
		ArgumentNullException.ThrowIfNull(stream);

		var xmlContent = SerializeXmlModel(model);
		await XmlDataRepository.SaveToStreamAsync(xmlContent, stream, cancellationToken);
	}

	/// <inheritdoc />
	public TacoMarkerPackModel ParseTacoMarkerPack(XDocument document)
	{
		ArgumentNullException.ThrowIfNull(document);
		return DeserializeXmlDocument(document) ?? new TacoMarkerPackModel([], [], []);
	}

	private static TacoMarkerPackModel? DeserializeXmlDocument(XDocument doc)
	{
		try
		{
			using var reader = doc.CreateReader();
			return Serializer.Deserialize(reader) is TacoOverlayDataDto dto ? TacoMapper.MapToDomain(dto) : null;
		}
		catch
		{
			return null;
		}
	}

	private static XDocument SerializeXmlModel(TacoMarkerPackModel model)
	{
		var dto = TacoMapper.MapToDto(model);

		// Avoid string-based serialization (extra allocations). Serialize directly into an XDocument.
		var doc = new XDocument();
		using var writer = doc.CreateWriter();
		Serializer.Serialize(writer, dto);
		return doc;
	}
}