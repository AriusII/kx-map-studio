namespace KXMapStudio.Core.Services.Serializations;

public sealed record XmlService(IXmlDataRepository XmlDataRepository) : IXmlService
{
	private static readonly XmlSerializer Serializer = new(typeof(TacoOverlayDataDto));

	public async Task<TacoMarkerPackModel?> LoadFromFileAsync(string filePath,
		CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

		var doc = await XmlDataRepository.LoadFromFileAsync(filePath, cancellationToken);
		return doc is null
			? null
			: DeserializeXmlDocument(doc);
	}

	public async Task SaveToFileAsync(TacoMarkerPackModel model, string filePath,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(model);
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

		var xmlContent = SerializeXmlModel(model);
		await XmlDataRepository.SaveToFileAsync(xmlContent, filePath, cancellationToken);
	}

	public async Task<TacoMarkerPackModel?> LoadFromStreamAsync(Stream stream,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(stream);

		var doc = await XmlDataRepository.LoadFromStreamAsync(stream, cancellationToken);
		return doc is null
			? null
			: DeserializeXmlDocument(doc);
	}

	public async Task SaveToStreamAsync(TacoMarkerPackModel model, Stream stream,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(model);
		ArgumentNullException.ThrowIfNull(stream);

		var xmlContent = SerializeXmlModel(model);
		await XmlDataRepository.SaveToStreamAsync(xmlContent, stream, cancellationToken);
	}

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
		using var writer = new StringWriter();
		Serializer.Serialize(writer, dto);
		return XDocument.Parse(writer.ToString());
	}
}