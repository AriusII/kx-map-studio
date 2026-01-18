namespace KXMapStudio.Libs.ViewModels.Workspace.LeftSide;

public sealed partial class FileExplorerViewModel : ObservableObject, IFileExplorerViewModel, IDisposable
{
	private readonly IFileExplorerNodeService _nodeService;
	private readonly Lock _refreshLock = new();
	private readonly FileSystemWatcher _watcher;
	private readonly IWorkshopExplorerService _workshopExplorerService;
	private int _isRefreshing;

	private string? _pendingSelectedPath;
	private Timer? _refreshTimer;

	[ObservableProperty] private WorkspaceExplorerNodeModel? _selectedNode;

	public FileExplorerViewModel(IWorkshopExplorerService workshopExplorerService, IFileExplorerNodeService nodeService)
	{
		_workshopExplorerService = workshopExplorerService;
		_nodeService = nodeService;

		RootNodes = [];
		RefreshTree();

		SelectNodeCommand = new RelayCommand<RoutedPropertyChangedEventArgs<object>>(OnSelectedItemChanged);

		_watcher = new FileSystemWatcher(_workshopExplorerService.DataFolder)
		{
			IncludeSubdirectories = true,
			EnableRaisingEvents = true,
			NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite
		};

		_watcher.Created += OnFsChanged;
		_watcher.Deleted += OnFsChanged;
		_watcher.Changed += OnFsChanged;
		_watcher.Renamed += OnFsRenamed;
	}

	public ObservableCollection<WorkspaceExplorerNodeModel> RootNodes { get; }

	public IRelayCommand<RoutedPropertyChangedEventArgs<object>> SelectNodeCommand { get; }

	public void Dispose()
	{
		_watcher.Created -= OnFsChanged;
		_watcher.Deleted -= OnFsChanged;
		_watcher.Changed -= OnFsChanged;
		_watcher.Renamed -= OnFsRenamed;
		_watcher.Dispose();

		lock (_refreshLock)
		{
			_refreshTimer?.Stop();
			_refreshTimer?.Dispose();
			_refreshTimer = null;
		}
	}

	public event EventHandler<string>? FileSelected;

	private void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object>? e)
	{
		if (e?.NewValue is not WorkspaceExplorerNodeModel node)
			return;

		SelectedNode = node;
	}

	partial void OnSelectedNodeChanged(WorkspaceExplorerNodeModel? value)
	{
		_pendingSelectedPath = value?.FullPath;

		if (value is null || value.IsDirectory)
			return;

		if (!_workshopExplorerService.IsAllowedFilePath(value.FullPath))
			return;

		FileSelected?.Invoke(this, value.FullPath);
	}

	private void OnFsChanged(object sender, FileSystemEventArgs e)
	{
		if (!_workshopExplorerService.IsRelevantChange(e.FullPath))
			return;

		QueueRefresh();
	}

	private void OnFsRenamed(object sender, RenamedEventArgs e)
	{
		if (_workshopExplorerService.IsRelevantChange(e.FullPath) ||
		    _workshopExplorerService.IsRelevantChange(e.OldFullPath))
			QueueRefresh();
	}

	private void QueueRefresh(int debounceMs = 250)
	{
		lock (_refreshLock)
		{
			_refreshTimer ??= new Timer { AutoReset = false };
			_refreshTimer.Interval = debounceMs;
			_refreshTimer.Elapsed -= RefreshTimerOnElapsed;
			_refreshTimer.Elapsed += RefreshTimerOnElapsed;
			_refreshTimer.Stop();
			_refreshTimer.Start();
		}
	}

	private void RefreshTimerOnElapsed(object? sender, ElapsedEventArgs e)
	{
		var app = Application.Current;
		app?.Dispatcher.Invoke(RefreshTree);
	}

	private void RefreshTree()
	{
		if (Interlocked.Exchange(ref _isRefreshing, 1) == 1)
			return;

		try
		{
			_pendingSelectedPath ??= SelectedNode?.FullPath;

			var root = _workshopExplorerService.BuildRootNode();
			root.IsExpanded = true;

			RootNodes.Clear();
			RootNodes.Add(root);

			WorkspaceExplorerNodeModel? toSelect = null;

			if (!string.IsNullOrWhiteSpace(_pendingSelectedPath))
			{
				toSelect = _workshopExplorerService.FindNodeByPath(root, _pendingSelectedPath!);
				if (toSelect != null)
					_nodeService.ExpandParents(root, toSelect.FullPath);
			}

			SelectedNode = toSelect ?? root;
			_pendingSelectedPath = null;
		}
		finally
		{
			Interlocked.Exchange(ref _isRefreshing, 0);
		}
	}
}