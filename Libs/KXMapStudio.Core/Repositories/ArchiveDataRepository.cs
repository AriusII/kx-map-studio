namespace KXMapStudio.Core.Repositories;

public sealed record ArchiveDataRepository : IArchiveDataRepository
{
	public async Task<IEnumerable<string>> ListContentsAsync(byte[] archiveData,
		CancellationToken cancellationToken = default)
	{
		using var memoryStream = new MemoryStream(archiveData);
		await using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read);

		return await Task.FromResult(archive.Entries.Select(e => e.FullName).ToList());
	}

	public async Task<Stream?> GetEntryStreamAsync(byte[] archiveData, string entryPath,
		CancellationToken cancellationToken = default)
	{
		var memoryStream = new MemoryStream(archiveData);
		var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read);

		var entry = archive.GetEntry(entryPath);
		if (entry == null)
		{
			await archive.DisposeAsync();
			await memoryStream.DisposeAsync();
			return null;
		}

		var entryStream = await entry.OpenAsync(cancellationToken);
		var ms = new MemoryStream();
		await entryStream.CopyToAsync(ms, cancellationToken);
		ms.Position = 0;

		await entryStream.DisposeAsync();
		await archive.DisposeAsync();
		await memoryStream.DisposeAsync();

		return ms;
	}
}