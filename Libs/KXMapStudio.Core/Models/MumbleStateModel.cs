namespace KXMapStudio.Core.Models;

/// <summary>
///     Represents a snapshot of Guild Wars 2 MumbleLink state.
/// </summary>
/// <param name="IsAvailable">Indicates whether MumbleLink data is currently available.</param>
/// <param name="PlayerPosition">The player avatar world position.</param>
/// <param name="CameraPosition">The camera world position.</param>
/// <param name="CurrentMapId">The current map identifier.</param>
/// <param name="CharacterName">The current character name.</param>
/// <param name="Timestamp">The UTC timestamp when the snapshot was captured.</param>
public sealed record MumbleStateModel(
	bool IsAvailable,
	Coordinates3 PlayerPosition,
	Coordinates3 CameraPosition,
	uint CurrentMapId,
	string CharacterName,
	DateTimeOffset Timestamp
);