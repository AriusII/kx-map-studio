namespace KXMapStudio.Libs.Abstractions;

public interface IGridDataService
{
	Task<IReadOnlyList<GridRowDto>> LoadRowsAsync(string path, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<GridRowDto>> LoadRowsAsync(string path, string category,
		CancellationToken cancellationToken = default);}