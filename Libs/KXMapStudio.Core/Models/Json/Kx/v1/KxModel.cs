namespace KXMapStudio.Core.Models.Json.Kx.v1;

public sealed record KxModel(
	[property: JsonPropertyName("Name")] string Name,
	[property: JsonPropertyName("Coordinates")]
	CoordinatesModel[] Coordinates
);