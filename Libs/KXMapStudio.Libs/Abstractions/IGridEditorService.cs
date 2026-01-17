namespace KXMapStudio.Libs.Abstractions;

/// <summary>
///     Service for managing grid editor operations including save functionality.
/// </summary>
public interface IGridEditorService
{
	/// <summary>
	///     Saves grid rows to the specified file path.
	/// </summary>
	Task SaveAsync(string filePath, string nodePath, IReadOnlyList<GridRowDto> rows,
		CancellationToken cancellationToken = default);
}