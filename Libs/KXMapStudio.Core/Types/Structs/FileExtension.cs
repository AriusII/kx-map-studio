namespace KXMapStudio.Core.Types.Structs;

public readonly ref struct FileExtension
{
	public static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
	{
		".zip",
		".json",
		".xml",
		".taco"
	};
}