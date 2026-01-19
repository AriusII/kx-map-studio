using KXMapStudio.Libs.Abstractions.ViewModels.TopSide.MainMenu;

namespace KXMapStudio.Libs.ViewModels.TopSide.MainMenu;

public sealed class MainMenuViewModel : ObservableObject, IMainMenuViewModel
{
	private readonly IGridEditorViewModel _gridEditor;

	public MainMenuViewModel(IGridEditorViewModel gridEditor)
	{
		_gridEditor = gridEditor ?? throw new ArgumentNullException(nameof(gridEditor));

		SaveCommand = _gridEditor.SaveCommand;
		SaveAsCommand = _gridEditor.SaveAsCommand;
		UndoCommand = _gridEditor.UndoCommand;
		RedoCommand = _gridEditor.RedoCommand;
	}

	public IAsyncRelayCommand SaveCommand { get; }
	public IAsyncRelayCommand SaveAsCommand { get; }
	public IRelayCommand UndoCommand { get; }
	public IRelayCommand RedoCommand { get; }

	public void Dispose()
	{
		// GridEditor lifetime is owned by DI; nothing to dispose here.
	}
}