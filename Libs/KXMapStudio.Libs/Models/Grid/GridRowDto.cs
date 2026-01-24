namespace KXMapStudio.Libs.Models.Grid;

/// <summary>
///     Represents a lightweight, serializable grid row.
/// </summary>
/// <param name="Id">The row identifier.</param>
/// <param name="Name">The row display name.</param>
/// <param name="X">The X coordinate.</param>
/// <param name="Y">The Y coordinate.</param>
/// <param name="Z">The Z coordinate.</param>
public sealed record GridRowDto(
	int Id,
	string Name,
	double X,
	double Y,
	double Z);