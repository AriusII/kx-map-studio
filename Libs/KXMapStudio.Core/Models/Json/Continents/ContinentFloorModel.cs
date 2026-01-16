namespace KXMapStudio.Core.Models.Json.Continents;

public sealed record ContinentFloorModel(
	[property: JsonPropertyName("id")] int Id,
	[property: JsonPropertyName("texture_dims")]
	int[] TextureDimensions,
	[property: JsonPropertyName("clamped_view")]
	int[][]? ClampedView,
	[property: JsonPropertyName("regions")]
	Dictionary<int, RegionModel> Regions);