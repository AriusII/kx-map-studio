namespace KXMapStudio.Libs.Abstractions.ViewModels.LeftSide;

public interface IFileExplorerViewModel : IDisposable
{
	ObservableCollection<WorkspaceExplorerNodeModel> RootNodes { get; }
	WorkspaceExplorerNodeModel? SelectedNode { get; set; }

	IRelayCommand<RoutedPropertyChangedEventArgs<object>> SelectNodeCommand { get; }
}