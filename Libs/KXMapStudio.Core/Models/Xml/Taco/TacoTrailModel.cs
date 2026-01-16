namespace KXMapStudio.Core.Models.Xml.Taco;

/// <summary>
///     Represents a Trail in a TacO marker pack.
/// </summary>
public sealed record TacoTrailModel(
	string Guid,
	string Type,
	string TrailData,
	string Texture)
{
	public float? AnimSpeed { get; init; }
	public float? FadeNear { get; init; }
	public float? FadeFar { get; init; }
}