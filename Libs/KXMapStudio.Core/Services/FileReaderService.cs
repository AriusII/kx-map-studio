namespace KXMapStudio.Core.Services;

/// <summary>
///     Provides high-level file workflows by delegating to format-specific services (XML/JSON/Archive).
/// </summary>
/// <param name="ArchiveService">The archive service.</param>
/// <param name="XmlService">The XML service.</param>
/// <param name="JsonService">The JSON service.</param>
public sealed record FileReaderService(
	IArchiveService ArchiveService,
	IXmlService XmlService,
	IJsonService JsonService)
	: IFileReaderService
{
	/// <inheritdoc />
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

	/// <inheritdoc />
	public Task<TacoMarkerPackModel?> ReadTacoAsync(
		string archiveFilePath,
		FileType fileType,
		string entryFullName,
		CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(archiveFilePath);
		ArgumentException.ThrowIfNullOrWhiteSpace(entryFullName);

		return fileType switch
		{
			FileType.Zip or FileType.Taco => ArchiveService.LoadMarkerPackAsync(archiveFilePath, entryFullName,
				cancellationToken),
			_ => throw new ArgumentOutOfRangeException(nameof(fileType), fileType,
				"Unsupported file type for archive entry load.")
		};
	}

	/// <inheritdoc />
	public Task<IReadOnlyList<MapModel>> ReadMapsJsonAsync(string filePath,
		CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
		return JsonService.LoadGuildWarsMapsAsync(filePath, cancellationToken);
	}

	/// <inheritdoc />
	public Task<ContinentFloorModel?> ReadContinentsJsonAsync(string filePath,
		CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
		return JsonService.LoadGuildWarsContinentFloorAsync(filePath, cancellationToken);
	}

	/// <inheritdoc />
	public Task<JsonModel> ReadJsonAsync(string filePath, CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
		return JsonService.LoadAsync(filePath, cancellationToken);
	}
}