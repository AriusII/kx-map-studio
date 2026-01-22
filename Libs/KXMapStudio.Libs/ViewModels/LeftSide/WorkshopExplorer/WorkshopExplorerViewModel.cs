namespace KXMapStudio.Libs.ViewModels.LeftSide.WorkshopExplorer;

public sealed partial class WorkshopExplorerViewModel : ObservableObject, IWorkshopExplorerViewModel
{
	private readonly HashSet<string> _expandedFolderPaths = new(StringComparer.OrdinalIgnoreCase);
	private readonly IWorkshopExplorerNodeService _nodeService;
	private readonly Lock _refreshLock = new();
	private readonly FileSystemWatcher _watcher;
	private readonly IWorkshopExplorerService _workshopExplorerService;
	[ObservableProperty] private bool _isRefreshing;
	[ObservableProperty] private string? _lastErrorMessage;
	[ObservableProperty] private DateTimeOffset? _lastRefreshUtc;
	private string? _pendingSelectedPath;

	private CancellationTokenSource? _refreshCts;
	private Timer? _refreshTimer;

	[ObservableProperty] private WorkspaceExplorerNodeModel? _selectedNode;

	public WorkshopExplorerViewModel(
		IWorkshopExplorerService workshopExplorerService,
		IWorkshopExplorerNodeService nodeService)
	{
		_workshopExplorerService = workshopExplorerService;
		_nodeService = nodeService;

		RootNodes = [];

		SelectNodeCommand = new RelayCommand<RoutedPropertyChangedEventArgs<object>>(OnSelectedItemChanged);
		RefreshCommand = new AsyncRelayCommand(RefreshAsync);

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

		// Initial load.
		QueueRefresh(0);
	}

	public ObservableCollection<WorkspaceExplorerNodeModel> RootNodes { get; }

	public IRelayCommand RefreshCommand { get; }
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

		CancelAndDisposeRefreshCts();
	}

	public event EventHandler<EditorDocumentReference>? FileSelected;

	private void CancelAndDisposeRefreshCts()
	{
		CancellationTokenSource? toDispose;
		lock (_refreshLock)
		{
			toDispose = _refreshCts;
			_refreshCts = null;
		}

		if (toDispose is null)
			return;

		try
		{
			toDispose.Cancel();
		}
		catch
		{
			/* ignore */
		}

		toDispose.Dispose();
	}

	private void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object>? e)
	{
		if (e?.NewValue is not WorkspaceExplorerNodeModel node)
			return;

		SelectedNode = node;
	}

	partial void OnSelectedNodeChanged(WorkspaceExplorerNodeModel? value)
	{
		_pendingSelectedPath = value?.FullPath;

		// Record expansion state when user expands/collapses nodes (two-way binding updates IsExpanded).
		if (value is { IsDirectory: true })
			CaptureExpandedStateFromRoots();

		if (value is null || value.IsDirectory)
			return;

		if (!_workshopExplorerService.IsAllowedFilePath(value.FullPath))
			return;

		FileSelected?.Invoke(this, EditorDocumentReference.FromWorkspaceFile(value.FullPath));
	}

	private void CaptureExpandedStateFromRoots()
	{
		_expandedFolderPaths.Clear();
		foreach (var root in RootNodes)
			_nodeService.TraverseTree(root, node =>
			{
				if (node.IsDirectory && node.IsExpanded)
					_expandedFolderPaths.Add(node.FullPath);
			});
	}

	private void RestoreExpandedStateFromSnapshot(WorkspaceExplorerNodeModel node)
	{
		_nodeService.TraverseTree(node, n =>
		{
			n.IsExpanded = n.IsDirectory && _expandedFolderPaths.Contains(n.FullPath);
		});
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
		// System.Timers.Timer.Interval must be > 0. For an immediate refresh request (startup),
		// schedule it directly on the dispatcher without going through the timer.
		if (debounceMs <= 0)
		{
			var app = Application.Current;
			if (app is null)
				return;

			_ = app.Dispatcher.InvokeAsync(async () =>
			{
				try
				{
					await RefreshAsync();
				}
				catch
				{
					// RefreshAsync is defensive; ignore any unexpected failures here.
				}
			});

			return;
		}

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
		// The timer is not on the UI thread. Schedule an async refresh back on the UI context.
		var app = Application.Current;
		if (app is null)
			return;

		_ = app.Dispatcher.InvokeAsync(async () =>
		{
			try
			{
				await RefreshAsync();
			}
			catch
			{
				// RefreshAsync is already defensive; this is a final safety net.
			}
		});
	}

	private async Task RefreshAsync()
	{
		CancellationToken token;
		lock (_refreshLock)
		{
			_pendingSelectedPath ??= SelectedNode?.FullPath;
		}

		// Snapshot expansion state before rebuilding the tree.
		CaptureExpandedStateFromRoots();

		lock (_refreshLock)
		{
			// Cancel previous refresh if any (VS Code behavior: last request wins).
			_refreshCts?.Cancel();
			_refreshCts?.Dispose();
			_refreshCts = new CancellationTokenSource();
			token = _refreshCts.Token;
		}

		IsRefreshing = true;
		LastErrorMessage = null;

		try
		{
			var root = await Task.Run(() => _workshopExplorerService.BuildRootNode(), token);

			// Restore expansion state like VS Code.
			RestoreExpandedStateFromSnapshot(root);
			root.IsExpanded = true;

			RootNodes.Clear();
			RootNodes.Add(root);

			WorkspaceExplorerNodeModel? toSelect = null;
			var path = _pendingSelectedPath;
			if (!string.IsNullOrWhiteSpace(path))
			{
				toSelect = _workshopExplorerService.FindNodeByPath(root, path);
				if (toSelect != null)
					_nodeService.ExpandParents(root, toSelect.FullPath);
			}

			SelectedNode = toSelect ?? root;
			_pendingSelectedPath = null;

			LastRefreshUtc = DateTimeOffset.UtcNow;
		}
		catch (OperationCanceledException)
		{
			// Expected in the "last refresh wins" model.
		}
		catch (Exception ex)
		{
			LastErrorMessage = ex.Message;
		}
		finally
		{
			IsRefreshing = false;
		}
	}
}