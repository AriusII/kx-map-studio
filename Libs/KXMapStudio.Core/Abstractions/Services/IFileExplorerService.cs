namespace KXMapStudio.Core.Abstractions.Services;

/// <summary>
///     Defines enumeration and tree-building of entries from the application data root.
/// </summary>
public interface IFileExplorerService
{
	/// <summary>
	///     Enumerates file system entries under the data root.
	/// </summary>
	IEnumerable<FileSystemEntryModel> EnumerateEntries(bool recursive = true);

	/// <summary>
	///     Builds a stable tree of entries rooted at the data root.
	/// </summary>
	FileSystemEntryNodeModel BuildTree(bool recursive = true);
}