namespace KXMapStudio.Core.Models.Json.Continents;

public sealed record RegionModel(
	[property: JsonPropertyName("id")] int Id,
	[property: JsonPropertyName("name")] string Name,
	[property: JsonPropertyName("label_coord")]
	double[] LabelCoord,
	[property: JsonPropertyName("continent_rect")]
	double[][] ContinentRect,
	[property: JsonPropertyName("maps")] Dictionary<int, MapInfoModel> Maps);