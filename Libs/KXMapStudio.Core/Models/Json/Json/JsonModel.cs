namespace KXMapStudio.Core.Models.Json.Json;

public sealed record JsonModel(
	[property: JsonPropertyName("Name")] string Name,
	[property: JsonPropertyName("Coordinates")]
	CoordinatesModel[] Coordinates
);