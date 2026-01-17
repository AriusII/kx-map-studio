namespace KXMapStudio.Core.Abstractions.Services.Serializations;

public interface IArchiveService
{
	Task<IReadOnlyList<string>> GetEntriesAsync(string filePath, CancellationToken cancellationToken = default);

	Task<TacoMarkerPackModel?> LoadMarkerPackAsync(
		string filePath,
		string? entryFullName = null,
		CancellationToken cancellationToken = default);
}