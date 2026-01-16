namespace KXMapStudio.Core.Models.Json.Continents;

public sealed record PointOfInterestModel(
	[property: JsonPropertyName("id")] int Id,
	[property: JsonPropertyName("name")] string? Name,
	[property: JsonPropertyName("type")] string Type,
	[property: JsonPropertyName("floor")] int? Floor,
	[property: JsonPropertyName("coord")] double[] Coord,
	[property: JsonPropertyName("chat_link")]
	string ChatLink,
	[property: JsonPropertyName("icon")] string? Icon);