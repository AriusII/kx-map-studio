namespace KXMapStudio.Core.Services.Serializations;

/// <summary>
///     Implementation of <see cref="ITacoService" /> using repositories for archive and XML handling.
/// </summary>
public sealed record TacoService(
	IArchiveRepository ArchiveRepository,
	IXmlDataRepository XmlDataRepository) : ITacoService
{
	public async Task<IEnumerable<string>> GetXmlFilesAsync(byte[] tacoData,
		CancellationToken cancellationToken = default)
	{
		var contents = await ArchiveRepository.ListContentsAsync(tacoData, cancellationToken);
		return contents.Where(c => c.EndsWith(".xml", StringComparison.OrdinalIgnoreCase));
	}

	public async Task<XDocument?> LoadXmlFileAsync(byte[] tacoData, string fileName,
		CancellationToken cancellationToken = default)
	{
		await using var stream = await ArchiveRepository.GetEntryStreamAsync(tacoData, fileName, cancellationToken);

		if (stream == null) return null;

		return await XmlDataRepository.LoadAsync(stream, cancellationToken);
	}

	public TacoMarkerPack ParseMarkerPack(XDocument document)
	{
		try
		{
			var serializer = new XmlSerializer(typeof(TacoOverlayDataDto));
			using var reader = document.CreateReader();
			var dto = (TacoOverlayDataDto?)serializer.Deserialize(reader);

			return dto != null
				? TacoMapper.MapToDomain(dto)
				: new TacoMarkerPack([], [], []);
		}
		catch (Exception)
		{
			return new TacoMarkerPack([], [], []);
		}
	}
}