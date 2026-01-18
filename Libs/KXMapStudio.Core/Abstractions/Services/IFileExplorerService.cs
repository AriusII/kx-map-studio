namespace KXMapStudio.Core.Abstractions.Services;

/// <summary>
///     Defines enumeration of entries from the application data root.
/// </summary>
public interface IFileExplorerService
{
	/// <summary>
	///     Enumerates file system entries under the data root.
	/// </summary>
	/// <param name="recursive">When <see langword="true" />, traverses subdirectories recursively.</param>
	/// <returns>A sequence of file system entries.</returns>
	IEnumerable<FileSystemEntryModel> EnumerateEntries(bool recursive = true);
}