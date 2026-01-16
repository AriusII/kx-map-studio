namespace KXMapStudio.Core.Models.Xml.Taco;

/// <summary>
///     Represents a Point of Interest (POI) or Marker in a TacO marker pack.
/// </summary>
public sealed record TacoPoi(
	string Guid,
	int MapId,
	float X,
	float Y,
	float Z,
	string Type)
{
	public string? IconFile { get; init; }
	public float? IconSize { get; init; }
}