namespace KXMapStudio.Core.Abstractions.Services.Serializations;

/// <summary>
///     Defines archive-centric workflows for ZIP/TACO containers.
/// </summary>
public interface IArchiveService
{
	/// <summary>
	///     Gets all entry full names in the specified archive.
	/// </summary>
	/// <param name="filePath">The archive file path.</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	/// <returns>A read-only list of entry names.</returns>
	Task<IReadOnlyList<string>> GetEntriesAsync(string filePath, CancellationToken cancellationToken = default);

	/// <summary>
	///     Loads a TACO marker pack from the specified archive.
	/// </summary>
	/// <param name="filePath">The archive file path.</param>
	/// <param name="entryFullName">
	///     The entry to load. When <see langword="null" />, an entry is auto-selected (typically the first <c>.xml</c> entry).
	/// </param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	/// <returns>The parsed marker pack, or <see langword="null" /> when missing or invalid.</returns>
	Task<TacoMarkerPackModel?> LoadMarkerPackAsync(string filePath, string? entryFullName = null,
		CancellationToken cancellationToken = default);
}