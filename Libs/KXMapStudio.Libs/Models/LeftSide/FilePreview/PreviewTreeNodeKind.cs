namespace KXMapStudio.Libs.Models.LeftSide.FilePreview;

/// <summary>
///     Represents the logical kind of a preview tree node.
/// </summary>
public enum PreviewTreeNodeKind
{
	/// <summary>Represents a file system directory when previewing an archive.</summary>
	ArchiveFolder,

	/// <summary>Represents a file entry inside an archive.</summary>
	ArchiveEntry,

	/// <summary>Represents a virtual node created from a parsed XML document (DTO-driven).</summary>
	XmlNode
}