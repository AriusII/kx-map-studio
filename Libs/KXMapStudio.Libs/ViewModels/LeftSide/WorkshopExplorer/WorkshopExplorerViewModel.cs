using KXMapStudio.Core.Abstractions.Repositories.FileStorage;

namespace KXMapStudio.Libs.ViewModels.LeftSide.WorkshopExplorer;

/// <summary>
///     Workshop Explorer ViewModel - unified file viewer for the Data folder.
/// </summary>
public sealed partial class WorkshopExplorerViewModel : ObservableObject, IWorkshopExplorerViewModel
{
	// Services
	private readonly ISaveFileDialogService _dialogService;

	// State
	private readonly HashSet<string> _expandedFolderPaths = new(StringComparer.OrdinalIgnoreCase);
	private readonly IFileStorageRepository _fileStorageRepository;
	private readonly Timer _refreshTimer;

	// File system monitoring
	private readonly FileSystemWatcher _watcher;
	private readonly IWorkshopExplorerService _workshopExplorerService;

	// Observable properties
	[ObservableProperty] private bool _isRefreshing;
	[ObservableProperty] private string? _lastErrorMessage;
	[ObservableProperty] private DateTimeOffset? _lastRefreshUtc;
	private CancellationTokenSource? _refreshCts;


	public WorkshopExplorerViewModel(
		IWorkshopExplorerService workshopExplorerService,
		IFileStorageRepository fileStorageRepository,
		ISaveFileDialogService dialogService)
	{
		_workshopExplorerService = workshopExplorerService;
		_fileStorageRepository = fileStorageRepository;
		_dialogService = dialogService;

		RootNodes = [];
		RefreshCommand = new AsyncRelayCommand(RefreshAsync);
		SelectNodeCommand = new RelayCommand<RoutedPropertyChangedEventArgs<object>>(OnSelectNode);
		CreateFileCommand = new RelayCommand<WorkshopExplorerNodeModel?>(OnCreateFile, CanCreateFile);
		DeleteFileCommand = new RelayCommand<WorkshopExplorerNodeModel?>(OnDeleteFile, CanDeleteFile);

		_watcher = new FileSystemWatcher(_workshopExplorerService.DataFolder)
		{
			IncludeSubdirectories = true,
			EnableRaisingEvents = true,
			NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite
		};

		_watcher.Created += OnFileSystemChanged;
		_watcher.Deleted += OnFileSystemChanged;
		_watcher.Changed += OnFileSystemChanged;
		_watcher.Renamed += OnFileSystemRenamed;

		_refreshTimer = new Timer { AutoReset = false };
		_refreshTimer.Elapsed += OnRefreshTimerElapsed;

		// Initial load on UI thread
		_ = Application.Current?.Dispatcher.InvokeAsync(async () => await RefreshAsync());
	}

	public ObservableCollection<WorkshopExplorerNodeModel> RootNodes { get; }
	public IRelayCommand RefreshCommand { get; }
	public IRelayCommand<RoutedPropertyChangedEventArgs<object>> SelectNodeCommand { get; }
	public IRelayCommand<WorkshopExplorerNodeModel?> CreateFileCommand { get; }
	public IRelayCommand<WorkshopExplorerNodeModel?> DeleteFileCommand { get; }

	public void Dispose()
	{
		_watcher.Created -= OnFileSystemChanged;
		_watcher.Deleted -= OnFileSystemChanged;
		_watcher.Changed -= OnFileSystemChanged;
		_watcher.Renamed -= OnFileSystemRenamed;
		_watcher.Dispose();

		_refreshTimer.Elapsed -= OnRefreshTimerElapsed;
		_refreshTimer.Dispose();

		_refreshCts?.Cancel();
		_refreshCts?.Dispose();
	}

	public event EventHandler<EditorDocumentReference>? FileSelected;

	private void OnFileSystemChanged(object sender, FileSystemEventArgs e)
	{
		if (_workshopExplorerService.IsRelevantChange(e.FullPath))
			QueueRefresh(250);
	}

	private void OnFileSystemRenamed(object sender, RenamedEventArgs e)
	{
		if (_workshopExplorerService.IsRelevantChange(e.FullPath) ||
		    _workshopExplorerService.IsRelevantChange(e.OldFullPath))
			QueueRefresh(250);
	}

	private void QueueRefresh(int debounceMs)
	{
		_refreshTimer.Stop();
		_refreshTimer.Interval = debounceMs;
		_refreshTimer.Start();
	}

	private void OnRefreshTimerElapsed(object? sender, ElapsedEventArgs e)
	{
		Application.Current?.Dispatcher.InvokeAsync(async () =>
		{
			try
			{
				await RefreshAsync();
			}
			catch
			{
				// RefreshAsync handles errors internally
			}
		});
	}

	private async Task RefreshAsync()
	{
		// Cancel previous refresh if still running
		_refreshCts?.Cancel();
		_refreshCts?.Dispose();
		_refreshCts = new CancellationTokenSource();

		var token = _refreshCts.Token;

		// Save current expanded state before clearing
		SaveExpandedState(RootNodes);

		IsRefreshing = true;
		LastErrorMessage = null;

		try
		{
			var scanResult =
				await _workshopExplorerService.ScanDirectoryAsync(_workshopExplorerService.DataFolder, token);

			// Build UI tree
			RootNodes.Clear();
			foreach (var childScan in scanResult.Children)
			{
				var childNode = BuildNodeRecursive(childScan);
				RootNodes.Add(childNode);
			}

			// Restore expanded state
			RestoreExpandedState(RootNodes);

			LastRefreshUtc = DateTimeOffset.UtcNow;
		}
		catch (OperationCanceledException)
		{
			// Expected when refresh is superseded
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

	private static WorkshopExplorerNodeModel BuildNodeRecursive(WorkshopExplorerScanNode scanNode)
	{
		var node = new WorkshopExplorerNodeModel(scanNode.Name, scanNode.FullPath, scanNode.IsDirectory);

		foreach (var childScan in scanNode.Children)
		{
			var childNode = BuildNodeRecursive(childScan);
			node.Children.Add(childNode);
		}

		return node;
	}

	private void SaveExpandedState(IEnumerable<WorkshopExplorerNodeModel> nodes)
	{
		foreach (var node in nodes)
		{
			if (node is { IsExpanded: true, IsDirectory: true })
				_expandedFolderPaths.Add(node.FullPath);

			SaveExpandedState(node.Children);
		}
	}

	private void RestoreExpandedState(IEnumerable<WorkshopExplorerNodeModel> nodes)
	{
		foreach (var node in nodes)
		{
			if (node.IsDirectory && _expandedFolderPaths.Contains(node.FullPath))
				node.IsExpanded = true;

			RestoreExpandedState(node.Children);
		}
	}

	private void OnSelectNode(RoutedPropertyChangedEventArgs<object>? e)
	{
		if (e?.NewValue is not WorkshopExplorerNodeModel node)
			return;

		// Only handle file selection (not directories)
		if (node.IsDirectory)
			return;

		// Only handle XML and JSON files
		var ext = node.Extension.ToLowerInvariant();
		if (ext != FileExtension.Json)
			return;

		// Fire event to notify subscribers (LeftSidePanelViewModel will handle loading into FilePreview and GridEditor)
		var doc = EditorDocumentReference.FromWorkspaceFile(node.FullPath);
		FileSelected?.Invoke(this, doc);
	}

	private bool CanCreateFile(WorkshopExplorerNodeModel? node)
	{
		// Allow creation at root level (node == null) or in directories
		return node == null || node.IsDirectory;
	}

	private async void OnCreateFile(WorkshopExplorerNodeModel? node)
	{
		// Determine target folder
		var targetFolder = GetTargetFolder(node);

		// Show dialog to create either XML or JSON
		var newFilePath = await _dialogService.ShowCreateFileDialogAsync(targetFolder, "xml");

		if (string.IsNullOrWhiteSpace(newFilePath))
			return;

		try
		{
			await CreateFileBasedOnExtension(newFilePath);
			// Refresh will be triggered automatically by FileSystemWatcher
		}
		catch (Exception ex)
		{
			LastErrorMessage = $"Failed to create file: {ex.Message}";
		}
	}

	private string GetTargetFolder(WorkshopExplorerNodeModel? node)
	{
		if (node == null)
			return _workshopExplorerService.DataFolder;

		return node.IsDirectory
			? node.FullPath
			: Path.GetDirectoryName(node.FullPath) ?? _workshopExplorerService.DataFolder;
	}

	private async Task CreateFileBasedOnExtension(string filePath)
	{
		var ext = Path.GetExtension(filePath).ToLowerInvariant();

		switch (ext)
		{
			case ".xml":
				break;
			case ".json":
				break;
			default:
				throw new NotSupportedException($"File type '{ext}' is not supported for creation.");
		}
	}

	private bool CanDeleteFile(WorkshopExplorerNodeModel? node)
	{
		// Only allow deleting files (not directories) that exist
		return node is { IsDirectory: false } && File.Exists(node.FullPath);
	}

	private async void OnDeleteFile(WorkshopExplorerNodeModel? node)
	{
		if (node == null || !ValidateFileExists(node.FullPath))
			return;

		var fileName = Path.GetFileName(node.FullPath);
		if (!await ConfirmFileDeletion(fileName))
			return;

		try
		{
			await _fileStorageRepository.DeleteFileAsync(node.FullPath);
			// Refresh will be triggered automatically by FileSystemWatcher
		}
		catch (Exception ex)
		{
			LastErrorMessage = $"Failed to delete file: {ex.Message}";
		}
	}

	private static bool ValidateFileExists(string filePath)
	{
		return File.Exists(filePath);
	}

	private async Task<bool> ConfirmFileDeletion(string fileName)
	{
		return await _dialogService.ShowDeleteFileConfirmationAsync(fileName);
	}
}