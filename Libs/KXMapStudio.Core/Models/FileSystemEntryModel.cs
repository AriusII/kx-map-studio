namespace KXMapStudio.Core.Models;

/// <summary>
///     Represents a file system entry returned by Core file exploration workflows.
/// </summary>
/// <param name="Type">The entry type (root/folder/file/archive).</param>
/// <param name="FullPath">The absolute path on disk.</param>
/// <param name="Name">The display name of the entry.</param>
/// <param name="RelativePath">The relative path from the data root.</param>
/// <param name="Extension">The file extension, if applicable.</param>
public sealed record FileSystemEntryModel(
	ExplorerEntryType Type,
	string FullPath,
	string Name,
	string RelativePath,
	string Extension
);