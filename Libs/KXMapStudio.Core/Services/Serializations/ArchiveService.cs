namespace KXMapStudio.Core.Services.Serializations;

public sealed record ArchiveService(
	IArchiveDataRepository ArchiveRepository,
	IXmlService XmlService)
	: IArchiveService
{
	public Task<IReadOnlyList<string>> GetEntriesAsync(string filePath,
		CancellationToken cancellationToken = default)
	{
		return ArchiveRepository.GetEntriesAsync(filePath, cancellationToken);
	}

	public async Task<TacoMarkerPackModel?> LoadMarkerPackAsync(
		string filePath,
		string? entryFullName = null,
		CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

		await using var archive = await ArchiveRepository.LoadAsync(filePath, cancellationToken);

		var entry = ResolveEntry(archive, entryFullName);
		if (entry is null) return null;

		await using var entryStream = await entry.OpenAsync(cancellationToken);
		return await XmlService.LoadFromStreamAsync(entryStream, cancellationToken);
	}

	private static ZipArchiveEntry? ResolveEntry(ZipArchive archive, string? entryFullName)
	{
		if (!string.IsNullOrWhiteSpace(entryFullName)) return archive.GetEntry(entryFullName);

		return archive.Entries
			.FirstOrDefault(e =>
				e.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase));
	}
}