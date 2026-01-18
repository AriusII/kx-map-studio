namespace KXMapStudio.Core.Types.Enums;

/// <summary>
///     Represents the kind of an entry returned by the file explorer.
/// </summary>
public enum ExplorerEntryType : byte
{
	/// <summary>
	///     The virtual root entry.
	/// </summary>
	Root,

	/// <summary>
	///     A directory.
	/// </summary>
	Folder,

	/// <summary>
	///     An archive file (*.zip or *.taco).
	/// </summary>
	Archive,

	/// <summary>
	///     A regular file.
	/// </summary>
	File
}