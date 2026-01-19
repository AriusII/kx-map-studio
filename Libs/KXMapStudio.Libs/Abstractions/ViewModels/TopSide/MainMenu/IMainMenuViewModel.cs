namespace KXMapStudio.Libs.Abstractions.ViewModels.TopSide.MainMenu;

public interface IMainMenuViewModel : IDisposable
{
	IAsyncRelayCommand SaveCommand { get; }
	IAsyncRelayCommand SaveAsCommand { get; }
	IRelayCommand UndoCommand { get; }
	IRelayCommand RedoCommand { get; }
}