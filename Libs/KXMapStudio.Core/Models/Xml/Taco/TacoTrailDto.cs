namespace KXMapStudio.Core.Models.Xml.Taco;

/// <summary>
///     DTO for the Trail element.
/// </summary>
public class TacoTrailDto
{
	[XmlAttribute("GUID")] public string? Guid { get; set; }

	[XmlAttribute("type")] public string? Type { get; set; }

	[XmlAttribute("trailData")] public string? TrailData { get; set; }

	[XmlAttribute("texture")] public string? Texture { get; set; }

	[XmlAttribute("animSpeed")] public string? AnimSpeed { get; set; }

	[XmlAttribute("fadeNear")] public string? FadeNear { get; set; }

	[XmlAttribute("fadeFar")] public string? FadeFar { get; set; }
}