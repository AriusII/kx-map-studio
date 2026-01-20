namespace KXMapStudio.Core.Models.Serializations.Xml;

/// <summary>
///     Represents a trail entry in a TacO marker pack.
/// </summary>
/// <param name="Guid">The trail GUID string.</param>
/// <param name="Type">The trail type.</param>
/// <param name="TrailData">The encoded trail polyline data.</param>
/// <param name="Texture">The texture identifier.</param>
public sealed record TrailModel(
	string Guid,
	string Type,
	string TrailData,
	string Texture)
{
	/// <summary>
	///     Gets the optional animation speed.
	/// </summary>
	public float? AnimSpeed { get; init; }

	/// <summary>
	///     Gets the optional near fade distance.
	/// </summary>
	public float? FadeNear { get; init; }

	/// <summary>
	///     Gets the optional far fade distance.
	/// </summary>
	public float? FadeFar { get; init; }
}