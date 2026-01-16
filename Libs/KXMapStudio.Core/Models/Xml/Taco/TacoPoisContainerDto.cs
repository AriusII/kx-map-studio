namespace KXMapStudio.Core.Models.Xml.Taco;

/// <summary>
///     DTO for the POIs container element.
/// </summary>
public class TacoPoisContainerDto
{
	[XmlElement("POI")] public List<TacoPoiDto> Pois { get; set; } = new();

	[XmlElement("Trail")] public List<TacoTrailDto> Trails { get; set; } = new();
}