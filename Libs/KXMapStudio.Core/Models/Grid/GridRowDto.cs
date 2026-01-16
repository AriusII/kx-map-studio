namespace KXMapStudio.Core.Models.Grid;

public sealed record GridRowDto(
	int Id,
	string Name,
	double X,
	double Y,
	double Z);