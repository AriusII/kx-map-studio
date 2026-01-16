namespace KXMapStudio.Core.Models.Json.Maps;

public sealed record MapModel(
	[property: JsonPropertyName("id")] int Id,
	[property: JsonPropertyName("name")] string Name,
	[property: JsonPropertyName("min_level")]
	int MinLevel,
	[property: JsonPropertyName("max_level")]
	int MaxLevel,
	[property: JsonPropertyName("default_floor")]
	int DefaultFloor,
	[property: JsonPropertyName("type")] string Type,
	[property: JsonPropertyName("floors")] int[] Floors,
	[property: JsonPropertyName("region_id")]
	int RegionId,
	[property: JsonPropertyName("region_name")]
	string RegionName,
	[property: JsonPropertyName("continent_id")]
	int ContinentId,
	[property: JsonPropertyName("continent_name")]
	string ContinentName,
	[property: JsonPropertyName("map_rect")]
	int[][] MapRect,
	[property: JsonPropertyName("continent_rect")]
	int[][] ContinentRect);