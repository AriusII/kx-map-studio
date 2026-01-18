namespace KXMapStudio.Core.Models.Xml.Taco.Dtos;

/// <summary>
///     Represents the root <c>OverlayData</c> XML element used by TacO marker packs.
/// </summary>
[XmlRoot("OverlayData")]
public sealed class TacoOverlayDataDto
{
	/// <summary>
	///     Gets or sets the list of marker categories.
	/// </summary>
	[XmlElement("MarkerCategory")]
	public List<TacoMarkerCategoryDto> Categories { get; set; } = [];

	/// <summary>
	///     Gets or sets the POIs container element.
	/// </summary>
	[XmlElement("POIs")]
	public TacoPoisContainerDto? PoisContainer { get; set; }
}