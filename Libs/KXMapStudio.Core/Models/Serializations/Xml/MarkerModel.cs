namespace KXMapStudio.Core.Models.Serializations.Xml;

/// <summary>
///     Represents the content of a TacO marker pack.
/// </summary>
/// <param name="Categories">The marker category hierarchy.</param>
/// <param name="Pois">The list of points of interest.</param>
/// <param name="Trails">The list of trail definitions.</param>
public sealed record MarkerModel(
	IReadOnlyList<MarkerCategoryModel> Categories,
	IReadOnlyList<PoiModel> Pois,
	IReadOnlyList<TrailModel> Trails);