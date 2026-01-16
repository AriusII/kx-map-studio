namespace KXMapStudio.Core.Models.Json.Continents;

public sealed record SkillChallengeModel(
	[property: JsonPropertyName("id")] string Id,
	[property: JsonPropertyName("coord")] double[] Coord);