namespace KXMapStudio.Core.Abstractions.Repositories.Serializations;

/// <summary>
///     Defines a persistence boundary for ZIP/TACO archives.
/// </summary>
public interface IArchiveDataRepository
{
	/// <summary>
	///     Opens an archive file for reading.
	/// </summary>
	/// <param name="filePath">The archive file path.</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	/// <returns>An opened <see cref="ZipArchive" /> instance.</returns>
	Task<ZipArchive> LoadAsync(string filePath, CancellationToken cancellationToken = default);

	/// <summary>
	///     Gets a list of all entry full names in the specified archive.
	/// </summary>
	/// <param name="filePath">The archive file path.</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	/// <returns>A read-only list of entry names.</returns>
	Task<IReadOnlyList<string>> GetEntriesAsync(string filePath, CancellationToken cancellationToken = default);

	/// <summary>
	///     Saves raw archive bytes to disk.
	/// </summary>
	/// <param name="archiveData">The raw archive payload.</param>
	/// <param name="filePath">The destination file path.</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	Task SaveAsync(byte[] archiveData, string filePath, CancellationToken cancellationToken = default);
}