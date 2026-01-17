namespace KXMapStudio.Core.Abstractions.Repositories;

public interface IFileStorageRepository
{
	Task SaveAsync(string path, Stream content, CancellationToken cancellationToken = default);
	Task<Stream?> LoadAsync(string path, CancellationToken cancellationToken = default);
	bool Exists(string path);
}