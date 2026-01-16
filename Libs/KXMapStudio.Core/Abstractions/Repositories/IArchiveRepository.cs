namespace KXMapStudio.Core.Abstractions.Repositories;

public interface IArchiveRepository
{
	Task<IEnumerable<string>> ListContentsAsync(byte[] archiveData, CancellationToken cancellationToken = default);

	Task<Stream?> GetEntryStreamAsync(byte[] archiveData, string entryPath,
		CancellationToken cancellationToken = default);
}