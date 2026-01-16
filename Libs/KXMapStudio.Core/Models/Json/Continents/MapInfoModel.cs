namespace KXMapStudio.Core.Models.Json.Continents;

public sealed record MapInfoModel(
	[property: JsonPropertyName("id")] int Id,
	[property: JsonPropertyName("name")] string Name,
	[property: JsonPropertyName("min_level")]
	int MinLevel,
	[property: JsonPropertyName("max_level")]
	int MaxLevel,
	[property: JsonPropertyName("default_floor")]
	int DefaultFloor,
	[property: JsonPropertyName("label_coord")]
	double[]? LabelCoord,
	[property: JsonPropertyName("map_rect")]
	double[][] MapRect,
	[property: JsonPropertyName("continent_rect")]
	double[][] ContinentRect,
	[property: JsonPropertyName("points_of_interest")]
	Dictionary<int, PointOfInterestModel> PointsOfInterest,
	[property: JsonPropertyName("tasks")] Dictionary<int, TaskInfoModel> Tasks,
	[property: JsonPropertyName("sectors")]
	Dictionary<int, SectorModel> Sectors,
	[property: JsonPropertyName("skill_challenges")]
	List<SkillChallengeModel> SkillChallenges,
	[property: JsonPropertyName("adventures")]
	List<AdventureModel> Adventures,
	[property: JsonPropertyName("mastery_points")]
	List<MasteryPointModel> MasteryPoints);