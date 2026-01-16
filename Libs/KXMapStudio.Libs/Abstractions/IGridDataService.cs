namespace KXMapStudio.Libs.Abstractions;

public interface IGridDataService
{
	Task<IReadOnlyList<GridRowDto>> LoadRowsAsync(string path, CancellationToken cancellationToken = default);
}