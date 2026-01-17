namespace KXMapStudio.Core.Repositories;

public sealed record FileStorageRepository : IFileStorageRepository
{
	public async Task SaveAsync(string path, Stream content, CancellationToken cancellationToken = default)
	{
		var directory = Path.GetDirectoryName(path);
		if (!string.IsNullOrEmpty(directory))
			Directory.CreateDirectory(directory);

		await using var fileStream =
			new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
		await content.CopyToAsync(fileStream, cancellationToken);
	}

	public Task<Stream?> LoadAsync(string path, CancellationToken cancellationToken = default)
	{
		return Task.FromResult<Stream?>(!Exists(path)
			? null
			: new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true));
	}

	public bool Exists(string path)
	{
		return File.Exists(path);
	}
}