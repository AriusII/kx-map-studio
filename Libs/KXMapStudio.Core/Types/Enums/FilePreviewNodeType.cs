namespace KXMapStudio.Core.Types.Enums;

/// <summary>
///     Defines the type of node in the file preview tree.
/// </summary>
public enum FilePreviewNodeType : byte
{
	/// <summary>
	///     Root node representing the entire file.
	/// </summary>
	Root,

	/// <summary>
	///     A file within an archive (e.g., .xml or .png inside .taco).
	/// </summary>
	ArchiveFile,

	/// <summary>
	///     A folder within an archive.
	/// </summary>
	ArchiveFolder,

	/// <summary>
	///     A data category (e.g., "Waypoints", "POI", "Trails").
	/// </summary>
	Category
}