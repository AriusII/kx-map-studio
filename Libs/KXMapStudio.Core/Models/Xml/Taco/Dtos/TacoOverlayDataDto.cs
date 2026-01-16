namespace KXMapStudio.Core.Models.Xml.Taco.Dtos;

/// <summary>
///     DTO for the root OverlayData element.
/// </summary>
[XmlRoot("OverlayData")]
public sealed class TacoOverlayDataDto
{
	[XmlElement("MarkerCategory")] public List<TacoMarkerCategoryDto> Categories { get; set; } = new();

	[XmlElement("POIs")] public TacoPoisContainerDto? PoisContainer { get; set; }
}