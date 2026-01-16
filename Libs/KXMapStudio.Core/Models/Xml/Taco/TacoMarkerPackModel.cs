namespace KXMapStudio.Core.Models.Xml.Taco;

/// <summary>
///     Represents the content of a TacO marker XML file.
/// </summary>
public sealed record TacoMarkerPackModel(
	List<TacoMarkerCategoryModel> Categories,
	List<TacoPoiModel> Pois,
	List<TacoTrailModel> Trails);