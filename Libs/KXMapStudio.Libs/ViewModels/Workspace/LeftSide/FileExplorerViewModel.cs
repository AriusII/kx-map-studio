namespace KXMapStudio.Libs.ViewModels.Workspace.LeftSide;

public sealed partial class FileExplorerViewModel : ObservableObject, IFileExplorerViewModel
{
	private readonly Lock _refreshLock = new();
	private readonly FileSystemWatcher _watcher;
	private readonly IWorkshopFileExplorerService _workshopFileExplorerService;
	private string? _pendingSelectedPath;
	private Timer? _refreshTimer;

	[ObservableProperty] private WorkspaceExplorerNodeModel? _selectedNode;

	public FileExplorerViewModel(IWorkshopFileExplorerService workshopFileExplorerService)
	{
		_workshopFileExplorerService = workshopFileExplorerService;

		RootNodes = [];
		RefreshTree();

		_watcher = new FileSystemWatcher(_workshopFileExplorerService.DataFolder)
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

	private void OnFsChanged(object sender, FileSystemEventArgs e)
	{
		if (!_workshopFileExplorerService.IsRelevantChange(e.FullPath))
			return;

		QueueRefresh();
	}

	private void OnFsRenamed(object sender, RenamedEventArgs e)
	{
		if (_workshopFileExplorerService.IsRelevantChange(e.FullPath) ||
		    _workshopFileExplorerService.IsRelevantChange(e.OldFullPath))
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
		// FileSystemWatcher events are not on the UI thread.
		var app = Application.Current;

		app?.Dispatcher.Invoke(RefreshTree);
	}

	private void RefreshTree()
	{
		_pendingSelectedPath ??= SelectedNode?.FullPath;

		var root = _workshopFileExplorerService.BuildRootNode();
		RootNodes.Clear();
		RootNodes.Add(root);

		if (!string.IsNullOrWhiteSpace(_pendingSelectedPath))
		{
			var found = _workshopFileExplorerService.FindNodeByPath(root, _pendingSelectedPath!);
			if (found != null)
			{
				ExpandParents(root, found.FullPath);
				SelectedNode = found;
			}
		}

		_pendingSelectedPath = null;
	}

	private static bool ExpandParents(WorkspaceExplorerNodeModel current, string targetFullPath)
	{
		if (string.Equals(current.FullPath, targetFullPath, StringComparison.OrdinalIgnoreCase))
			return true;

		foreach (var child in current.Children)
			if (ExpandParents(child, targetFullPath))
			{
				current.IsExpanded = true;
				return true;
			}

		return false;
	}
}