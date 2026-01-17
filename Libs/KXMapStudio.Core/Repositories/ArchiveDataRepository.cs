namespace KXMapStudio.Core.Repositories;

public sealed record ArchiveDataRepository(IFileStorageRepository FileStorage) : IArchiveDataRepository
{
	public async Task<ZipArchive> LoadAsync(string filePath, CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

		await using var stream = await FileStorage.LoadAsync(filePath, cancellationToken);

		return new ZipArchive(stream!, ZipArchiveMode.Read);
	}

	public async Task<IReadOnlyList<string>> GetEntriesAsync(string filePath,
		CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
		await using var archive = await LoadAsync(filePath, cancellationToken);
		return archive.Entries.Select(e => e.FullName).ToList();
	}

	public async Task SaveAsync(byte[] archiveData, string filePath,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(archiveData);
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

		await using var memory = new MemoryStream(archiveData, false);
		await FileStorage.SaveAsync(filePath, memory, cancellationToken);
	}
}