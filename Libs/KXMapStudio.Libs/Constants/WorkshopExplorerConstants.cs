namespace KXMapStudio.Libs.Constants;

/// <summary>
///     Constants used by the Workshop Explorer functionality.
/// </summary>
public static class WorkshopExplorerConstants
{
	/// <summary>
	///     File extensions allowed in the workshop explorer (currently XML and JSON).
	/// </summary>
	public static readonly IReadOnlyCollection<string> AllowedFileExtensions =
	[
		FileExtension.Xml,
		FileExtension.Json
	];

	/// <summary>
	///     Case-insensitive string comparer for file paths and extensions.
	/// </summary>
	public static readonly StringComparer PathComparer = StringComparer.OrdinalIgnoreCase;
}
