namespace KXMapStudio.Core.Models.Json.Continents;

public sealed record SectorModel(
	[property: JsonPropertyName("id")] int Id,
	[property: JsonPropertyName("name")] string Name,
	[property: JsonPropertyName("level")] int Level,
	[property: JsonPropertyName("coord")] double[] Coord,
	[property: JsonPropertyName("bounds")] double[][] Bounds,
	[property: JsonPropertyName("chat_link")]
	string ChatLink);