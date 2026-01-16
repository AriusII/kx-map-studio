using KXMapStudio.Core.Services;

namespace KXMapStudio.Libs.ViewModels.Workspace;

public sealed partial class LeftWorkspaceViewModel : ObservableObject, IDisposable
{
	private readonly IMessenger _messenger;
	private readonly DispatcherTimer _refreshDebounceTimer;
	private readonly IWorkspaceExplorerService _workspaceExplorerService;
	private readonly IFilePreviewService _filePreviewService;

	[ObservableProperty] [NotifyPropertyChangedFor(nameof(PreviewLoadTimeText))]
	private TimeSpan _previewLoadTime;

	[ObservableProperty] private bool _isPreviewBusy;
	[ObservableProperty] private string _previewStatus = "Aucun fichier sélectionné.";
	[ObservableProperty] private string? _openedFileName;
	[ObservableProperty] private string? _openedFilePath;

	[ObservableProperty] private FilePreviewCategoryModel? _selectedPreviewItem;

	[ObservableProperty] [NotifyPropertyChangedFor(nameof(PreviewText))]
	private WorkspaceExplorerNodeModel? _selectedNode;

	private FileSystemWatcher? _watcher;
	private CancellationTokenSource? _previewCts;

	public LeftWorkspaceViewModel()
		: this(new WorkspaceExplorerService(), CreateDefaultPreviewService(), WeakReferenceMessenger.Default)
	{
	}

	private LeftWorkspaceViewModel(
		IWorkspaceExplorerService workspaceExplorerService,
		IFilePreviewService filePreviewService,
		IMessenger messenger)
	{
		_workspaceExplorerService = workspaceExplorerService;
		_filePreviewService = filePreviewService;
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
	public ObservableCollection<FilePreviewCategoryModel> PreviewItems { get; } = [];

	public string PreviewText => SelectedNode is null
		? "No file selected."
		: $"Preview: {SelectedNode.Name}";

	public string PreviewLoadTimeText => PreviewLoadTime == TimeSpan.Zero
		? "-"
		: $"{PreviewLoadTime.TotalMilliseconds:0} ms";

	public void Dispose()
	{
		_refreshDebounceTimer.Stop();
		_watcher?.Dispose();
		_previewCts?.Cancel();
		_previewCts?.Dispose();
	}

	[RelayCommand]
	private async Task SelectNode(RoutedPropertyChangedEventArgs<object> args)
	{
		if (args.NewValue is not WorkspaceExplorerNodeModel { IsDirectory: false } node) return;
		SelectedNode = node;
		await LoadPreviewAsync(node.FullPath);
	}

	[RelayCommand]
	private void SelectPreviewCategory(FilePreviewCategoryModel? item)
	{
		if (item is null || string.IsNullOrWhiteSpace(OpenedFilePath)) return;

		foreach (var category in PreviewItems)
			category.IsSelected = ReferenceEquals(category, item);

		SelectedPreviewItem = item;
		_messenger.Send(new GridCategorySelectedMessageModel(OpenedFilePath, item.Name));
	}

	[RelayCommand]
	private void Refresh()
	{
		ReloadWorkspaceNodes();
	}

	private async Task LoadPreviewAsync(string path, CancellationToken cancellationToken = default)
	{
		_previewCts?.Cancel();
		_previewCts = new CancellationTokenSource();

		try
		{
			IsPreviewBusy = true;
			PreviewStatus = "Chargement...";
			PreviewItems.Clear();
			SelectedPreviewItem = null;

			var result = await _filePreviewService.BuildPreviewAsync(path, _previewCts.Token);

			OpenedFilePath = result.Path;
			OpenedFileName = Path.GetFileName(result.Path);
			PreviewLoadTime = result.LoadTime;

			foreach (var item in result.Categories)
				PreviewItems.Add(new FilePreviewCategoryModel(item.Name, item.Count));

			PreviewStatus = result.Categories.Count == 0
				? "Aucun élément détecté."
				: $"{result.Categories.Count} catégories disponibles.";
		}
		catch (OperationCanceledException)
		{
			PreviewStatus = "Chargement annulé.";
		}
		catch (Exception ex)
		{
			PreviewStatus = $"Erreur: {ex.Message}";
		}
		finally
		{
			IsPreviewBusy = false;
		}
	}

	private static IFilePreviewService CreateDefaultPreviewService()
	{
		var xmlRepo = new XmlDataRepository();
		var jsonRepo = new JsonDataRepository();
		var archiveRepo = new ArchiveDataRepository();

		var fileTypeDetector = new FileTypeDetectorService(jsonRepo, xmlRepo, archiveRepo);
		var xmlService = new XmlService();
		var jsonService = new JsonService(jsonRepo);
		var archiveService = new ArchiveService(archiveRepo, xmlRepo);

		return new FilePreviewService(fileTypeDetector, xmlService, archiveService, jsonService);
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