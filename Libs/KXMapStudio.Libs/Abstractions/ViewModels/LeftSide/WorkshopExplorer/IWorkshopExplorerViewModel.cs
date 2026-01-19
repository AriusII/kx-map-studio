namespace KXMapStudio.Libs.Abstractions.ViewModels.LeftSide.WorkshopExplorer;

public interface IWorkshopExplorerViewModel : IDisposable
{
	ObservableCollection<WorkspaceExplorerNodeModel> RootNodes { get; }
	WorkspaceExplorerNodeModel? SelectedNode { get; set; }

	IRelayCommand<RoutedPropertyChangedEventArgs<object>> SelectNodeCommand { get; }
}