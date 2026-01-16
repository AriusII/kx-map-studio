namespace KXMapStudio.Core.Models.Json.Kx.v1;

public sealed record CoordinatesModel(
	[property: JsonPropertyName("Name")] string Name,
	[property: JsonPropertyName("X")] double X,
	[property: JsonPropertyName("Y")] double Y,
	[property: JsonPropertyName("Z")] double Z
);