namespace KXMapStudio.Core.Models.Xml.Taco.Dtos;

/// <summary>
///     DTO for the POI element.
/// </summary>
public sealed class TacoPoiDto
{
	[XmlAttribute("GUID")] public string? Guid { get; set; }

	[XmlAttribute("MapID")] public string? MapId { get; set; }

	[XmlAttribute("xpos")] public string? XPos { get; set; }

	[XmlAttribute("ypos")] public string? YPos { get; set; }

	[XmlAttribute("zpos")] public string? ZPos { get; set; }

	[XmlAttribute("type")] public string? Type { get; set; }

	// Some POIs have additional attributes that override the category
	[XmlAttribute("iconFile")] public string? IconFile { get; set; }

	[XmlAttribute("iconSize")] public string? IconSize { get; set; }
}