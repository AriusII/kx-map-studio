namespace KXMapStudio.Core.Models.Xml.Taco;

/// <summary>
///     Represents a point-of-interest (POI) entry in a TacO marker pack.
/// </summary>
/// <param name="Guid">The marker GUID string.</param>
/// <param name="MapId">The map identifier.</param>
/// <param name="X">The X world position.</param>
/// <param name="Y">The Y world position.</param>
/// <param name="Z">The Z world position.</param>
/// <param name="Type">The marker type/category name.</param>
public sealed record TacoPoiModel(
	string Guid,
	int MapId,
	float X,
	float Y,
	float Z,
	string Type)
{
	/// <summary>
	///     Gets the optional icon file path.
	/// </summary>
	public string? IconFile { get; init; }

	/// <summary>
	///     Gets the optional icon size override.
	/// </summary>
	public float? IconSize { get; init; }
}