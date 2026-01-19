namespace KXMapStudio.Libs.Abstractions.ViewModels.RightSide.GridEditor;

public interface IGridEditorViewModel : IDisposable
{
	string? DocumentTitle { get; }
	bool IsLoaded { get; }
	bool IsDirty { get; }

	bool CanUndo { get; }
	bool CanRedo { get; }

	bool CanSave { get; }
	bool CanSaveAs { get; }

	IAsyncRelayCommand SaveCommand { get; }
	IAsyncRelayCommand SaveAsCommand { get; }
	IRelayCommand UndoCommand { get; }
	IRelayCommand RedoCommand { get; }
	IRelayCommand<GridEditorRowViewModel?> MoveUpCommand { get; }
	IRelayCommand<GridEditorRowViewModel?> MoveDownCommand { get; }

	ObservableCollection<GridEditorRowViewModel> Rows { get; }

	Task LoadAsync(EditorDocumentReference doc, CancellationToken cancellationToken = default);
}