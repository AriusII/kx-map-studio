namespace KXMapStudio.Core.Models.Grid;

/// <summary>
///     Immutable snapshot of grid document state for undo/redo operations.
/// </summary>
public sealed record GridDocumentState(
	string FilePath,
	string NodePath,
	IReadOnlyList<GridRowDto> Rows)
{
	public static GridDocumentState FromModels(string filePath, string nodePath, IEnumerable<GridRowModel> models)
	{
		return new GridDocumentState(filePath, nodePath, models.Select(m => m.ToDto()).ToList());
	}
}