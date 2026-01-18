namespace KXMapStudio.Core.Models.Xml.Taco.Dtos;

/// <summary>
///     Represents the <c>POIs</c> container element in a TacO marker pack.
/// </summary>
public sealed class TacoPoisContainerDto
{
	/// <summary>
	///     Gets or sets the list of POI elements.
	/// </summary>
	[XmlElement("POI")]
	public List<TacoPoiDto> Pois { get; set; } = [];

	/// <summary>
	///     Gets or sets the list of trail elements.
	/// </summary>
	[XmlElement("Trail")]
	public List<TacoTrailDto> Trails { get; set; } = [];
}