namespace KXMapStudio.Libs.ViewModels.Workspace;

public sealed partial class MainMenuViewModel : ObservableObject, IDisposable
{
	private readonly IMessenger _messenger;
	[ObservableProperty] private string _appVersion = "v1.0.0";
	[ObservableProperty] private bool _canRedo;
	[ObservableProperty] private bool _canSave;

	[ObservableProperty] private bool _canUndo;
	[ObservableProperty] private object? _globalHotkeys;

	public MainMenuViewModel() : this(WeakReferenceMessenger.Default)
	{
	}

	private MainMenuViewModel(IMessenger messenger)
	{
		_messenger = messenger;
		messenger.Register<EditorStateChangedMessage>(this, OnEditorStateChanged);
	}

	public void Dispose()
	{
		_messenger.UnregisterAll(this);
	}

	private void OnEditorStateChanged(object recipient, EditorStateChangedMessage message)
	{
		CanUndo = message.CanUndo;
		CanRedo = message.CanRedo;
		CanSave = message.CanSave;
	}

	[RelayCommand(CanExecute = nameof(CanSave))]
	private void SaveDocument()
	{
		_messenger.Send(new SaveRequestMessage());
	}

	[RelayCommand]
	private void SaveAs()
	{
		var dialog = new SaveFileDialog
		{
			Filter = "XML Files|*.xml|JSON Files|*.json|All Files|*.*",
			Title = "Save As"
		};

		if (dialog.ShowDialog() == true) _messenger.Send(new SaveAsRequestMessage(dialog.FileName));
	}

	[RelayCommand(CanExecute = nameof(CanUndo))]
	private void Undo()
	{
		_messenger.Send(new UndoRequestMessage());
	}

	[RelayCommand(CanExecute = nameof(CanRedo))]
	private void Redo()
	{
		_messenger.Send(new RedoRequestMessage());
	}

	[RelayCommand]
	private void MoveMarkersUp()
	{
		// Handled by GridEditor via selected row
	}

	[RelayCommand]
	private void MoveMarkersDown()
	{
		// Handled by GridEditor via selected row
	}

	[RelayCommand]
	private void AddMarkerFromGame()
	{
		// TODO: Implement hotkey functionality
	}

	[RelayCommand]
	private void UndoLastAddedMarker()
	{
		// TODO: Implement hotkey functionality
	}

	partial void OnCanSaveChanged(bool value)
	{
		SaveDocumentCommand.NotifyCanExecuteChanged();
	}

	partial void OnCanUndoChanged(bool value)
	{
		UndoCommand.NotifyCanExecuteChanged();
	}

	partial void OnCanRedoChanged(bool value)
	{
		RedoCommand.NotifyCanExecuteChanged();
	}
}