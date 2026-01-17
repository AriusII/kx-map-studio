namespace KXMapStudio.Core.Abstractions.Repositories.Serializations;

public interface IArchiveDataRepository
{
	Task<ZipArchive> LoadAsync(string filePath, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<string>> GetEntriesAsync(string filePath, CancellationToken cancellationToken = default);
	Task SaveAsync(byte[] archiveData, string filePath, CancellationToken cancellationToken = default);
}