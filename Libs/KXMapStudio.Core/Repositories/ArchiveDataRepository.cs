namespace KXMapStudio.Core.Repositories;

/// <summary>
///     Provides ZIP/TACO archive persistence and access.
/// </summary>
/// <param name="FileStorage">The underlying file storage abstraction.</param>
public sealed record ArchiveDataRepository(IFileStorageRepository FileStorage) : IArchiveDataRepository
{
	/// <inheritdoc />
	public async Task<ZipArchive> LoadAsync(string filePath, CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

		await using var stream = await FileStorage.LoadAsync(filePath, cancellationToken);
		if (stream is null)
			throw new FileNotFoundException("Archive file not found.", filePath);

		// The ZipArchive must remain usable after this method returns, so we do not dispose the underlying stream here.
		return new ZipArchive(stream, ZipArchiveMode.Read, false);
	}

	/// <inheritdoc />
	public async Task<IReadOnlyList<string>> GetEntriesAsync(string filePath,
		CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

		if (!FileStorage.Exists(filePath))
			return [];

		await using var archive = await LoadAsync(filePath, cancellationToken);
		return archive.Entries.Select(e => e.FullName).ToList();
	}

	/// <inheritdoc />
	public async Task SaveAsync(byte[] archiveData, string filePath,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(archiveData);
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

		await using var memory = new MemoryStream(archiveData, false);
		await FileStorage.SaveAsync(filePath, memory, cancellationToken);
	}
}