namespace KXMapStudio.Core.Abstractions.Services.Serializations;

public interface IArchiveService : IDisposable
{
	bool IsArchiveLoaded { get; }
	Task LoadArchiveFromFileAsync(string filePath, CancellationToken cancellationToken = default);
	Task LoadArchiveFromBytesAsync(byte[] archiveData, CancellationToken cancellationToken = default);
	IReadOnlyList<string> ListAllEntries();
	IReadOnlyList<string> ListXmlEntries();
	bool ContainsEntry(string entryPath);
	Task<Stream?> GetEntryStreamAsync(string entryPath, CancellationToken cancellationToken = default);
	Task<TacoMarkerPackModel?> LoadTacoMarkerPackFromEntryAsync(string xmlEntryPath, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<TacoMarkerPackModel>> LoadAllTacoMarkerPacksAsync(CancellationToken cancellationToken = default);
	Task AddOrUpdateXmlEntryAsync(string entryPath, TacoMarkerPackModel content, CancellationToken cancellationToken = default);
	Task AddOrUpdateEntryAsync(string entryPath, byte[] content, CancellationToken cancellationToken = default);
	Task RemoveEntryAsync(string entryPath, CancellationToken cancellationToken = default);
	Task SaveArchiveToFileAsync(string filePath, CancellationToken cancellationToken = default);
	Task<byte[]> GetArchiveBytesAsync(CancellationToken cancellationToken = default);
}