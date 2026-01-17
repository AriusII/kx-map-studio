namespace KXMapStudio.Core.Services.Serializations;

public sealed record ArchiveService(
	IArchiveDataRepository ArchiveDataRepository,
	IXmlDataRepository XmlDataRepository) : IArchiveService
{
	public async Task<IEnumerable<string>> GetXmlFilesAsync(byte[] tacoData,
		CancellationToken cancellationToken = default)
	{
		var contents = await ArchiveDataRepository.ListContentsAsync(tacoData, cancellationToken);
		return contents.Where(c => c.EndsWith(".xml", StringComparison.OrdinalIgnoreCase));
	}

	public async Task<XDocument?> LoadXmlFileAsync(byte[] tacoData, string fileName,
		CancellationToken cancellationToken = default)
	{
		await using var stream = await ArchiveDataRepository.GetEntryStreamAsync(tacoData, fileName, cancellationToken);

		if (stream == null) return null;

		return await XmlDataRepository.LoadFromArchiveAsync(stream, cancellationToken);
	}

	public TacoMarkerPackModel ParseMarkerPack(XDocument document)
	{
		try
		{
			var serializer = new XmlSerializer(typeof(TacoOverlayDataDto));
			using var reader = document.CreateReader();
			var dto = (TacoOverlayDataDto?)serializer.Deserialize(reader);

			return dto != null
				? TacoMapper.MapToDomain(dto)
				: new TacoMarkerPackModel([], [], []);
		}
		catch (Exception)
		{
			return new TacoMarkerPackModel([], [], []);
		}
	}

	public async Task<IReadOnlyList<TacoMarkerPackModel>> LoadMarkerPacksAsync(byte[] archiveData,
		CancellationToken cancellationToken = default)
	{
		var result = new List<TacoMarkerPackModel>();

		var xmlFiles = await GetXmlFilesAsync(archiveData, cancellationToken);
		foreach (var file in xmlFiles)
		{
			var doc = await LoadXmlFileAsync(archiveData, file, cancellationToken);
			if (doc == null) continue;

			result.Add(ParseMarkerPack(doc));
		}

		return result;
	}

	public async Task<IReadOnlyList<TacoMarkerPackModel>> LoadMarkerPacksAsync(string path,
		CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(path);
		if (!File.Exists(path)) return [];

		var bytes = await File.ReadAllBytesAsync(path, cancellationToken);
		return await LoadMarkerPacksAsync(bytes, cancellationToken);
	}
}