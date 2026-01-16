namespace KXMapStudio.Core.Models.Xml.Taco;

/// <summary>
///     Represents the content of a TacO marker XML file.
/// </summary>
public sealed record TacoMarkerPack(
	List<TacoMarkerCategory> Categories,
	List<TacoPoi> Pois,
	List<TacoTrail> Trails);