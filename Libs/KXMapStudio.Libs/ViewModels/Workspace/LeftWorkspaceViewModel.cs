namespace KXMapStudio.Libs.ViewModels.Workspace;

public sealed partial class LeftWorkspaceViewModel : ObservableObject, IDisposable
{
	private readonly IFilePreviewService _filePreviewService;
	private readonly IFileSystemService _fileSystemService;
	private readonly IMessenger _messenger;
	private readonly DispatcherTimer _refreshDebounceTimer;
	private readonly IWorkspaceExplorerService _workspaceExplorerService;

	[ObservableProperty] private bool _isPreviewBusy;
	[ObservableProperty] private string? _openedFileName;
	[ObservableProperty] private string? _openedFilePath;
	private CancellationTokenSource? _previewCts;

	[ObservableProperty] [NotifyPropertyChangedFor(nameof(PreviewLoadTimeText))]
	private TimeSpan _previewLoadTime;

	[ObservableProperty] private string _previewStatus = "Aucun fichier sélectionné.";

	[ObservableProperty] [NotifyPropertyChangedFor(nameof(PreviewText))]
	private WorkspaceExplorerNodeModel? _selectedNode;

	private FileSystemWatcher? _watcher;

	public LeftWorkspaceViewModel()
		: this(new WorkspaceExplorerService(), new FileSystemService(Path.Combine(AppContext.BaseDirectory, "Data")),
			CreateDefaultPreviewService(), WeakReferenceMessenger.Default)
	{
	}

	private LeftWorkspaceViewModel(
		IWorkspaceExplorerService workspaceExplorerService,
		IFileSystemService fileSystemService,
		IFilePreviewService filePreviewService,
		IMessenger messenger)
	{
		_workspaceExplorerService = workspaceExplorerService;
		_fileSystemService = fileSystemService;
		_filePreviewService = filePreviewService;
		_messenger = messenger;

		_refreshDebounceTimer = new DispatcherTimer
		{
			Interval = TimeSpan.FromMilliseconds(500)
		};

		if (IsDesignMode)
			return;

		_refreshDebounceTimer.Tick += (_, _) =>
		{
			_refreshDebounceTimer.Stop();
			Refresh();
		};

		ReloadWorkspaceNodes();
		InitializeWatcher();
	}

	private static bool IsDesignMode =>
		DesignerProperties.GetIsInDesignMode(new DependencyObject());

	public ObservableCollection<WorkspaceExplorerNodeModel> RootNodes { get; } = [];
	public ObservableCollection<FilePreviewTreeNodeModel> PreviewTreeNodes { get; } = [];

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
	private void SelectPreviewTreeNode(FilePreviewTreeNodeModel? node)
	{
		if (node is null || string.IsNullOrWhiteSpace(OpenedFilePath)) return;
		if (node.NodeType != FilePreviewNodeType.Category) return;
		if (string.IsNullOrWhiteSpace(node.FullPath)) return;

		_messenger.Send(new GridCategorySelectedMessageModel(OpenedFilePath, node.FullPath));
	}

	[RelayCommand]
	private void Refresh()
	{
		ReloadWorkspaceNodes();
	}

	private async Task LoadPreviewAsync(string path)
	{
		_previewCts?.Cancel();
		_previewCts = new CancellationTokenSource();

		try
		{
			IsPreviewBusy = true;
			PreviewStatus = "Chargement...";
			PreviewTreeNodes.Clear();

			var result = await _filePreviewService.BuildPreviewTreeAsync(path, _previewCts.Token);

			OpenedFilePath = result.Path;
			OpenedFileName = Path.GetFileName(result.Path);
			PreviewLoadTime = result.LoadTime;

			// Expand root node by default
			result.RootNode.IsExpanded = true;
			PreviewTreeNodes.Add(result.RootNode);

			var childrenCount = CountAllChildren(result.RootNode);
			PreviewStatus = childrenCount == 0
				? "Aucun élément détecté."
				: $"{childrenCount} éléments disponibles.";
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

	private static int CountAllChildren(FilePreviewTreeNodeModel node)
	{
		var count = node.Children.Count;
		foreach (var child in node.Children)
			count += CountAllChildren(child);
		return count;
	}

	private static FilePreviewService CreateDefaultPreviewService()
	{
		var xmlRepo = new XmlDataRepository();
		var jsonRepo = new JsonDataRepository();
		var archiveRepo = new ArchiveDataRepository();

		var fileTypeDetector = new FileTypeDetectorService(jsonRepo, xmlRepo, archiveRepo);
		var xmlService = new XmlService();
		var jsonService = new JsonService(jsonRepo);
		var archiveService = new ArchiveService(archiveRepo, xmlRepo);

		return new FilePreviewService(fileTypeDetector, xmlService, archiveService, archiveRepo, jsonService);
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

	[RelayCommand]
	private async Task CreateFile()
	{
		try
		{
			var targetPath = DetermineTargetDirectory();

			var dialog = new CreateFileDialog { Owner = Application.Current.MainWindow };
			if (dialog.ShowDialog() != true || string.IsNullOrWhiteSpace(dialog.FileName))
				return;

			await _fileSystemService.CreateFileAsync(targetPath, dialog.FileName, dialog.SelectedFileType);
		}
		catch (Exception ex)
		{
			MessageBox.Show($"Failed to create file: {ex.Message}", "Error", MessageBoxButton.OK,
				MessageBoxImage.Error);
		}
	}

	[RelayCommand]
	private async Task CreateFolder()
	{
		try
		{
			var targetPath = DetermineTargetDirectory();

			var dialog = new CreateFolderDialog { Owner = Application.Current.MainWindow };
			if (dialog.ShowDialog() != true || string.IsNullOrWhiteSpace(dialog.FolderName))
				return;

			await _fileSystemService.CreateDirectoryAsync(targetPath, dialog.FolderName);
		}
		catch (Exception ex)
		{
			MessageBox.Show($"Failed to create folder: {ex.Message}", "Error", MessageBoxButton.OK,
				MessageBoxImage.Error);
		}
	}

	[RelayCommand]
	private async Task DeleteSelected()
	{
		if (SelectedNode is null)
			return;

		try
		{
			if (_fileSystemService.IsRootDataFolder(SelectedNode.FullPath))
			{
				MessageBox.Show("Cannot delete the root Data folder.", "Invalid Operation", MessageBoxButton.OK,
					MessageBoxImage.Warning);
				return;
			}

			var itemType = SelectedNode.IsDirectory ? "folder" : "file";
			var result = MessageBox.Show(
				$"Are you sure you want to delete this {itemType}?\n\n{SelectedNode.Name}",
				"Confirm Delete",
				MessageBoxButton.YesNo,
				MessageBoxImage.Question);

			if (result != MessageBoxResult.Yes)
				return;

			if (SelectedNode.IsDirectory)
				await _fileSystemService.DeleteDirectoryAsync(SelectedNode.FullPath);
			else
				await _fileSystemService.DeleteFileAsync(SelectedNode.FullPath);

			SelectedNode = null;
		}
		catch (Exception ex)
		{
			MessageBox.Show($"Failed to delete: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}

	[RelayCommand]
	private async Task CreateFileInArchive(WorkspaceExplorerNodeModel? node)
	{
		if (node is null)
			return;

		try
		{
			var extension = Path.GetExtension(node.FullPath).ToLowerInvariant();
			if (extension != ".zip" && extension != ".taco")
			{
				MessageBox.Show("This action is only available for .zip and .taco archives.", "Invalid Operation",
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			var dialog = new CreateArchiveFileDialog { Owner = Application.Current.MainWindow };
			if (dialog.ShowDialog() != true || string.IsNullOrWhiteSpace(dialog.FileName))
				return;

			await _fileSystemService.CreateFileInArchiveAsync(node.FullPath, dialog.FileName);
			MessageBox.Show("File created successfully in archive.", "Success", MessageBoxButton.OK,
				MessageBoxImage.Information);
		}
		catch (Exception ex)
		{
			MessageBox.Show($"Failed to create file in archive: {ex.Message}", "Error", MessageBoxButton.OK,
				MessageBoxImage.Error);
		}
	}

	[RelayCommand]
	private async Task DeleteNode(WorkspaceExplorerNodeModel? node)
	{
		if (node is null)
			return;

		try
		{
			if (_fileSystemService.IsRootDataFolder(node.FullPath))
			{
				MessageBox.Show("Cannot delete the root Data folder.", "Invalid Operation", MessageBoxButton.OK,
					MessageBoxImage.Warning);
				return;
			}

			var itemType = node.IsDirectory ? "folder" : "file";
			var result = MessageBox.Show(
				$"Are you sure you want to delete this {itemType}?\n\n{node.Name}",
				"Confirm Delete",
				MessageBoxButton.YesNo,
				MessageBoxImage.Question);

			if (result != MessageBoxResult.Yes)
				return;

			if (node.IsDirectory)
				await _fileSystemService.DeleteDirectoryAsync(node.FullPath);
			else
				await _fileSystemService.DeleteFileAsync(node.FullPath);
		}
		catch (Exception ex)
		{
			MessageBox.Show($"Failed to delete: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}

	[RelayCommand]
	private async Task CreateFileInFolder(WorkspaceExplorerNodeModel? node)
	{
		if (node is null || !node.IsDirectory)
			return;

		try
		{
			var dialog = new CreateFileDialog { Owner = Application.Current.MainWindow };
			if (dialog.ShowDialog() != true || string.IsNullOrWhiteSpace(dialog.FileName))
				return;

			await _fileSystemService.CreateFileAsync(node.FullPath, dialog.FileName, dialog.SelectedFileType);
		}
		catch (Exception ex)
		{
			MessageBox.Show($"Failed to create file: {ex.Message}", "Error", MessageBoxButton.OK,
				MessageBoxImage.Error);
		}
	}

	[RelayCommand]
	private async Task CreateFolderInFolder(WorkspaceExplorerNodeModel? node)
	{
		if (node is null || !node.IsDirectory)
			return;

		try
		{
			var dialog = new CreateFolderDialog { Owner = Application.Current.MainWindow };
			if (dialog.ShowDialog() != true || string.IsNullOrWhiteSpace(dialog.FolderName))
				return;

			await _fileSystemService.CreateDirectoryAsync(node.FullPath, dialog.FolderName);
		}
		catch (Exception ex)
		{
			MessageBox.Show($"Failed to create folder: {ex.Message}", "Error", MessageBoxButton.OK,
				MessageBoxImage.Error);
		}
	}

	private string DetermineTargetDirectory()
	{
		if (SelectedNode is null)
			return _workspaceExplorerService.DataFolder;

		return SelectedNode.IsDirectory
			? SelectedNode.FullPath
			: Path.GetDirectoryName(SelectedNode.FullPath) ?? _workspaceExplorerService.DataFolder;
	}
}