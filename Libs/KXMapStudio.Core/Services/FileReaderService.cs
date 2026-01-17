namespace KXMapStudio.Core.Services;

public sealed record FileReaderService(
	IArchiveService ArchiveService,
	IXmlService XmlService,
	IJsonService JsonService)
	: IFileReaderService
{
	public async Task<TacoMarkerPackModel?> ReadTacoAsync(string filePath, FileType fileType,
		CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

		return fileType switch
		{
			FileType.Xml => await XmlService.LoadFromFileAsync(filePath, cancellationToken),
			FileType.Zip or FileType.Taco =>
				await ArchiveService.LoadMarkerPackAsync(filePath, null, cancellationToken),
			_ => throw new ArgumentOutOfRangeException(nameof(fileType), fileType, "Unsupported file type for TACO.")
		};
	}

	public Task<IReadOnlyList<MapModel>> ReadMapsJsonAsync(string filePath,
		CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
		return JsonService.LoadGuildWarsMapsAsync(filePath, cancellationToken);
	}

	public Task<ContinentFloorModel?> ReadContinentsJsonAsync(string filePath,
		CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
		return JsonService.LoadGuildWarsContinentFloorAsync(filePath, cancellationToken);
	}

	public Task<JsonModel> ReadJsonAsync(string filePath, CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
		return JsonService.LoadAsync(filePath, cancellationToken);
	}
}