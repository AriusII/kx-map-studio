namespace KXMapStudio.Core.Models.Json.Continents;

public sealed record MasteryPointModel(
	[property: JsonPropertyName("id")] int Id,
	[property: JsonPropertyName("region")] string Region,
	[property: JsonPropertyName("coord")] double[] Coord);