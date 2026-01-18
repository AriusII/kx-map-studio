namespace KXMapStudio.Core.Models.Xml.Taco.Dtos;

/// <summary>
///     Represents the <c>Trail</c> XML element in a TacO marker pack.
/// </summary>
public sealed class TacoTrailDto
{
	/// <summary>
	///     Gets or sets the GUID attribute.
	/// </summary>
	[XmlAttribute("GUID")]
	public string? Guid { get; set; }

	/// <summary>
	///     Gets or sets the trail type.
	/// </summary>
	[XmlAttribute("type")]
	public string? Type { get; set; }

	/// <summary>
	///     Gets or sets the encoded trail polyline.
	/// </summary>
	[XmlAttribute("trailData")]
	public string? TrailData { get; set; }

	/// <summary>
	///     Gets or sets the trail texture.
	/// </summary>
	[XmlAttribute("texture")]
	public string? Texture { get; set; }

	/// <summary>
	///     Gets or sets the animation speed.
	/// </summary>
	[XmlAttribute("animSpeed")]
	public string? AnimSpeed { get; set; }

	/// <summary>
	///     Gets or sets the near fade distance.
	/// </summary>
	[XmlAttribute("fadeNear")]
	public string? FadeNear { get; set; }

	/// <summary>
	///     Gets or sets the far fade distance.
	/// </summary>
	[XmlAttribute("fadeFar")]
	public string? FadeFar { get; set; }
}