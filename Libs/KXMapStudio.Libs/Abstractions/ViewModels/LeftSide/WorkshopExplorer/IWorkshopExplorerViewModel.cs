namespace KXMapStudio.Libs.Abstractions.ViewModels.LeftSide.WorkshopExplorer;

public interface IWorkshopExplorerViewModel : IDisposable
{
	ObservableCollection<WorkshopExplorerNodeModel> RootNodes { get; }
	bool IsRefreshing { get; }
	DateTimeOffset? LastRefreshUtc { get; }
	string? LastErrorMessage { get; }

	IRelayCommand RefreshCommand { get; }
	IRelayCommand<RoutedPropertyChangedEventArgs<object>> SelectNodeCommand { get; }
	IRelayCommand<WorkshopExplorerNodeModel?> CreateFileCommand { get; }
	IRelayCommand<WorkshopExplorerNodeModel?> DeleteFileCommand { get; }
}