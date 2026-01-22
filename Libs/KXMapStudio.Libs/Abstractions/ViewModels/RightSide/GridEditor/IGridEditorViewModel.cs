namespace KXMapStudio.Libs.Abstractions.ViewModels.RightSide.GridEditor;

public interface IGridEditorViewModel : IDisposable
{
	string? DocumentTitle { get; }
	bool IsLoaded { get; }
	bool IsDirty { get; }

	string? OpenedFileName { get; }
	string OpenedFileLoadTime { get; }
	string FileExtension { get; }

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
	IRelayCommand AddRowCommand { get; }
	IRelayCommand AddMarkerFromMumbleCommand { get; }

	ObservableCollection<GridEditorRowViewModel> Rows { get; }

	Task LoadAsync(EditorDocumentReference doc, CancellationToken cancellationToken = default);
}