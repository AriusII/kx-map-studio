namespace KXMapStudio.Core.Models.Grid;

/// <summary>
///     Represents an immutable snapshot of a grid document state for undo/redo operations.
/// </summary>
/// <param name="FilePath">The source file path.</param>
/// <param name="NodePath">The selected node path within the document.</param>
/// <param name="Rows">The grid rows snapshot.</param>
public sealed record GridDocumentState(
	string FilePath,
	string NodePath,
	IReadOnlyList<GridRowDto> Rows)
{
	/// <summary>
	///     Creates a snapshot from the current UI models.
	/// </summary>
	/// <param name="filePath">The source file path.</param>
	/// <param name="nodePath">The selected node path.</param>
	/// <param name="models">The current row models.</param>
	/// <returns>A new immutable snapshot.</returns>
	public static GridDocumentState FromModels(string filePath, string nodePath, IEnumerable<GridRowModel> models)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
		ArgumentException.ThrowIfNullOrWhiteSpace(nodePath);
		ArgumentNullException.ThrowIfNull(models);

		// Avoid LINQ allocations on hot paths.
		var rows = new List<GridRowDto>();
		foreach (var m in models)
			rows.Add(m.ToDto());

		return new GridDocumentState(filePath, nodePath, rows);
	}
}