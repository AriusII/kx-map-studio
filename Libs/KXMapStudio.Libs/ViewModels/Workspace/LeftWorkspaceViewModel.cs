namespace KXMapStudio.Libs.ViewModels.Workspace;

public sealed partial class LeftWorkspaceViewModel : ObservableObject, IDisposable
{
	private readonly IMessenger _messenger;
	private readonly DispatcherTimer _refreshDebounceTimer;
	private readonly IWorkspaceExplorerService _workspaceExplorerService;

	[ObservableProperty] [NotifyPropertyChangedFor(nameof(PreviewText))]
	private WorkspaceExplorerNodeModel? _selectedNode;

	private FileSystemWatcher? _watcher;

	public LeftWorkspaceViewModel()
		: this(new WorkspaceExplorerService(), WeakReferenceMessenger.Default)
	{
	}

	private LeftWorkspaceViewModel(IWorkspaceExplorerService workspaceExplorerService, IMessenger messenger)
	{
		_workspaceExplorerService = workspaceExplorerService;
		_messenger = messenger;

		_refreshDebounceTimer = new DispatcherTimer
		{
			Interval = TimeSpan.FromMilliseconds(500)
		};
		_refreshDebounceTimer.Tick += (_, _) =>
		{
			_refreshDebounceTimer.Stop();
			Refresh();
		};

		ReloadWorkspaceNodes();
		InitializeWatcher();
	}

	public ObservableCollection<WorkspaceExplorerNodeModel> RootNodes { get; } = [];

	public string PreviewText => SelectedNode is null
		? "No file selected."
		: $"Preview: {SelectedNode.Name}";

	public void Dispose()
	{
		_refreshDebounceTimer.Stop();
		_watcher?.Dispose();
	}

	[RelayCommand]
	private void SelectNode(RoutedPropertyChangedEventArgs<object> args)
	{
		if (args.NewValue is not WorkspaceExplorerNodeModel { IsDirectory: false } node) return;
		SelectedNode = node;
		_messenger.Send(new FileSelectedMessageModel(node.FullPath));
	}

	[RelayCommand]
	private void Refresh()
	{
		ReloadWorkspaceNodes();
	}

	private void ReloadWorkspaceNodes()
	{
		var selectedPath = SelectedNode?.FullPath;

		RootNodes.Clear();

		var rootNode = _workspaceExplorerService.BuildRootNode();
		rootNode.IsExpanded = true;

		RootNodes.Add(rootNode);

		if (selectedPath is null) return;
		var found = _workspaceExplorerService.FindNodeByPath(rootNode, selectedPath);
		if (found is not null) SelectedNode = found;
	}

	private void InitializeWatcher()
	{
		_watcher = new FileSystemWatcher(_workspaceExplorerService.DataFolder)
		{
			IncludeSubdirectories = true,
			NotifyFilter = NotifyFilters.FileName
			               | NotifyFilters.DirectoryName
			               | NotifyFilters.LastWrite
			               | NotifyFilters.Size
		};

		_watcher.Created += OnWorkspaceChanged;
		_watcher.Changed += OnWorkspaceChanged;
		_watcher.Deleted += OnWorkspaceChanged;
		_watcher.Renamed += OnWorkspaceChanged;

		_watcher.EnableRaisingEvents = true;
	}

	private void OnWorkspaceChanged(object? sender, FileSystemEventArgs e)
	{
		if (!_workspaceExplorerService.IsRelevantChange(e.FullPath)) return;

		_refreshDebounceTimer.Stop();
		_refreshDebounceTimer.Start();
	}
}