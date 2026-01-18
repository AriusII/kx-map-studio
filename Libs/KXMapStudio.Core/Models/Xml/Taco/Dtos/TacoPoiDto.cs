namespace KXMapStudio.Core.Models.Xml.Taco.Dtos;

/// <summary>
///     Represents the <c>POI</c> XML element in a TacO marker pack.
/// </summary>
public sealed class TacoPoiDto
{
	/// <summary>
	///     Gets or sets the GUID attribute.
	/// </summary>
	[XmlAttribute("GUID")]
	public string? Guid { get; set; }

	/// <summary>
	///     Gets or sets the map identifier.
	/// </summary>
	[XmlAttribute("MapID")]
	public string? MapId { get; set; }

	/// <summary>
	///     Gets or sets the X position.
	/// </summary>
	[XmlAttribute("xpos")]
	public string? XPos { get; set; }

	/// <summary>
	///     Gets or sets the Y position.
	/// </summary>
	[XmlAttribute("ypos")]
	public string? YPos { get; set; }

	/// <summary>
	///     Gets or sets the Z position.
	/// </summary>
	[XmlAttribute("zpos")]
	public string? ZPos { get; set; }

	/// <summary>
	///     Gets or sets the POI type.
	/// </summary>
	[XmlAttribute("type")]
	public string? Type { get; set; }

	/// <summary>
	///     Gets or sets an optional icon file override.
	/// </summary>
	[XmlAttribute("iconFile")]
	public string? IconFile { get; set; }

	/// <summary>
	///     Gets or sets an optional icon size override.
	/// </summary>
	[XmlAttribute("iconSize")]
	public string? IconSize { get; set; }
}