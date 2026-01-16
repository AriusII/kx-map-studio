namespace KXMapStudio.Core.Models.Json.Continents;

public sealed record TaskInfoModel(
	[property: JsonPropertyName("id")] int Id,
	[property: JsonPropertyName("objective")]
	string Objective,
	[property: JsonPropertyName("level")] int Level,
	[property: JsonPropertyName("coord")] double[] Coord);