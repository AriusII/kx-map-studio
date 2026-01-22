namespace KXMapStudio.Libs.Abstractions.ViewModels.LeftSide.WorkshopExplorer;

public interface IWorkshopExplorerViewModel : IDisposable
{
	ObservableCollection<WorkspaceExplorerNodeModel> RootNodes { get; }
	WorkspaceExplorerNodeModel? SelectedNode { get; set; }

	bool IsRefreshing { get; }
	DateTimeOffset? LastRefreshUtc { get; }
	string? LastErrorMessage { get; }

	IRelayCommand RefreshCommand { get; }
	IRelayCommand<RoutedPropertyChangedEventArgs<object>> SelectNodeCommand { get; }
}