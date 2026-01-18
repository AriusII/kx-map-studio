namespace KXMapStudio.Core.Types.Enums;

/// <summary>
///     Specifies the supported file types handled by Core workflows.
/// </summary>
public enum FileType : byte
{
	/// <summary>
	///     Unknown or unsupported file type.
	/// </summary>
	Unknown,

	/// <summary>
	///     A plain XML file.
	/// </summary>
	Xml,

	/// <summary>
	///     A ZIP archive.
	/// </summary>
	Zip,

	/// <summary>
	///     A TACO archive (ZIP-compatible).
	/// </summary>
	Taco,

	/// <summary>
	///     A JSON file.
	/// </summary>
	Json
}