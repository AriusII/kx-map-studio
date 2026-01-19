namespace KXMapStudio.Libs.Abstractions.Services.GridEditor;

public interface IGridEditorDocumentService
{
	Task<(IReadOnlyList<GridEditorRowViewModel> rows, byte[] originalBytes)> LoadAsync(
		EditorDocumentReference doc,
		CancellationToken cancellationToken = default);

	Task SaveAsync(
		EditorDocumentReference doc,
		IReadOnlyList<GridEditorRowViewModel> rows,
		CancellationToken cancellationToken = default);

	Task SaveAsAsync(
		EditorDocumentReference doc,
		IReadOnlyList<GridEditorRowViewModel> rows,
		CancellationToken cancellationToken = default);
}
