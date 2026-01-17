namespace KXMapStudio.Libs.ViewModels.Workspace;

public sealed partial class GridEditorViewModel : ObservableObject, IDisposable
{
	private readonly IGridDataService _gridDataService;
	private readonly IGridEditorService _gridEditorService;
	private readonly IStateHistoryService<GridDocumentState> _historyService;
	private readonly IMessenger _messenger;
	[ObservableProperty] private string _currentFilePath = string.Empty;
	[ObservableProperty] private string _currentNodePath = string.Empty;
	[ObservableProperty] private bool _hasUnsavedChanges;
	[ObservableProperty] private bool _isBusy;

	private CancellationTokenSource? _loadCts;

	[ObservableProperty] private GridRowModel? _selectedRow;
	[ObservableProperty] private string _status = "Aucun fichier sélectionné.";

	public GridEditorViewModel()
		: this(
			CreateDefaultGridDataService(),
			CreateDefaultGridEditorService(),
			new StateHistoryService<GridDocumentState>(),
			WeakReferenceMessenger.Default)
	{
	}

	private GridEditorViewModel(
		IGridDataService gridDataService,
		IGridEditorService gridEditorService,
		IStateHistoryService<GridDocumentState> historyService,
		IMessenger messenger)
	{
		_gridDataService = gridDataService;
		_gridEditorService = gridEditorService;
		_historyService = historyService;
		_messenger = messenger;

		messenger.Register<GridCategorySelectedMessageModel>(this,
			async void (_, message) => { await LoadFromPathAsync(message.Path, message.NodePath); });

		messenger.Register<UndoRequestMessage>(this, (_, _) => Undo());
		messenger.Register<RedoRequestMessage>(this, (_, _) => Redo());
		messenger.Register<SaveRequestMessage>(this, async void (_, _) => { await SaveAsync(); });
		messenger.Register<SaveAsRequestMessage>(this, async void (_, m) => { await SaveAsAsync(m.FilePath); });
	}

	public ObservableCollection<GridRowModel> Rows { get; } = [];

	public bool CanUndo => _historyService.CanUndo;
	public bool CanRedo => _historyService.CanRedo;
	public bool CanSave => !string.IsNullOrWhiteSpace(CurrentFilePath) && HasUnsavedChanges;

	public void Dispose()
	{
		_loadCts?.Dispose();
		foreach (var row in Rows)
			row.PropertyChanged -= OnRowPropertyChanged;
	}

	[RelayCommand(CanExecute = nameof(CanMoveUp))]
	private void MoveUp()
	{
		if (SelectedRow is null) return;

		var index = Rows.IndexOf(SelectedRow);
		if (index <= 0) return;

		SaveStateSnapshot();
		Rows.Move(index, index - 1);
		HasUnsavedChanges = true;
		NotifyCanExecuteChanged();
	}

	private bool CanMoveUp()
	{
		return SelectedRow is not null && Rows.IndexOf(SelectedRow) > 0;
	}

	[RelayCommand(CanExecute = nameof(CanMoveDown))]
	private void MoveDown()
	{
		if (SelectedRow is null) return;

		var index = Rows.IndexOf(SelectedRow);
		if (index < 0 || index >= Rows.Count - 1) return;

		SaveStateSnapshot();
		Rows.Move(index, index + 1);
		HasUnsavedChanges = true;
		NotifyCanExecuteChanged();
	}

	private bool CanMoveDown()
	{
		return SelectedRow is not null && Rows.IndexOf(SelectedRow) < Rows.Count - 1;
	}

	partial void OnSelectedRowChanged(GridRowModel? value)
	{
		MoveUpCommand.NotifyCanExecuteChanged();
		MoveDownCommand.NotifyCanExecuteChanged();
	}

	public void Undo()
	{
		var previousState = _historyService.Undo();
		if (previousState is null) return;

		ApplyState(previousState);
		NotifyCanExecuteChanged();
	}

	public void Redo()
	{
		var nextState = _historyService.Redo();
		if (nextState is null) return;

		ApplyState(nextState);
		NotifyCanExecuteChanged();
	}

	public async Task SaveAsync()
	{
		if (string.IsNullOrWhiteSpace(CurrentFilePath))
			return;

		await SaveToPathAsync(CurrentFilePath);
	}

	public async Task SaveAsAsync(string filePath)
	{
		if (string.IsNullOrWhiteSpace(filePath))
			return;

		await SaveToPathAsync(filePath);
		CurrentFilePath = filePath;
		NotifyCanExecuteChanged();
	}

	private async Task SaveToPathAsync(string filePath)
	{
		try
		{
			IsBusy = true;
			Status = "Sauvegarde en cours...";

			var rows = Rows.Select(r => r.ToDto()).ToList();
			await _gridEditorService.SaveAsync(filePath, CurrentNodePath, rows);

			HasUnsavedChanges = false;
			Status = "Sauvegarde réussie.";
			NotifyCanExecuteChanged();
		}
		catch (Exception ex)
		{
			Status = $"Erreur de sauvegarde: {ex.Message}";
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task LoadFromPathAsync(string path, string nodePath, CancellationToken cancellationToken = default)
	{
		_loadCts?.Cancel();
		_loadCts = new CancellationTokenSource();

		try
		{
			IsBusy = true;
			var categoryName = nodePath.Split('|', StringSplitOptions.RemoveEmptyEntries).LastOrDefault() ?? "données";
			Status = $"Chargement {categoryName}...";

			var dtos = await _gridDataService.LoadRowsAsync(path, nodePath, _loadCts.Token);

			Rows.Clear();
			foreach (var dto in dtos)
			{
				var model = GridRowModel.FromDto(dto);
				model.PropertyChanged += OnRowPropertyChanged;
				Rows.Add(model);
			}

			CurrentFilePath = path;
			CurrentNodePath = nodePath;
			HasUnsavedChanges = false;
			_historyService.Clear();
			SaveStateSnapshot();

			Status = dtos.Count == 0 ? "Aucune donnée." : $"{dtos.Count} lignes chargées.";
			NotifyCanExecuteChanged();
		}
		catch (OperationCanceledException)
		{
			Status = "Chargement annulé.";
		}
		catch (Exception ex)
		{
			Status = $"Erreur: {ex.Message}";
		}
		finally
		{
			IsBusy = false;
		}
	}

	private void OnRowPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (!HasUnsavedChanges)
		{
			SaveStateSnapshot();
			HasUnsavedChanges = true;
			NotifyCanExecuteChanged();
		}
	}

	private void SaveStateSnapshot()
	{
		if (string.IsNullOrWhiteSpace(CurrentFilePath))
			return;

		var state = GridDocumentState.FromModels(CurrentFilePath, CurrentNodePath, Rows.Select(r => r.Clone()));
		_historyService.PushState(state);
	}

	private void ApplyState(GridDocumentState state)
	{
		foreach (var row in Rows)
			row.PropertyChanged -= OnRowPropertyChanged;

		Rows.Clear();
		foreach (var dto in state.Rows)
		{
			var model = GridRowModel.FromDto(dto);
			model.PropertyChanged += OnRowPropertyChanged;
			Rows.Add(model);
		}

		HasUnsavedChanges = true;
	}

	private void NotifyCanExecuteChanged()
	{
		_messenger.Send(new EditorStateChangedMessage(CanUndo, CanRedo, CanSave));
	}

	private static GridDataService CreateDefaultGridDataService()
	{
		var xmlRepo = new XmlDataRepository();
		var jsonRepo = new JsonDataRepository();
		var archiveRepo = new ArchiveDataRepository();
		var fileTypeDetector = new FileTypeDetectorService(jsonRepo, xmlRepo, archiveRepo);
		var kxService = new JsonService(jsonRepo);

		return new GridDataService(fileTypeDetector, xmlRepo, jsonRepo, archiveRepo, kxService);
	}

	private static GridEditorService CreateDefaultGridEditorService()
	{
		var xmlRepo = new XmlDataRepository();
		var jsonRepo = new JsonDataRepository();
		var archiveRepo = new ArchiveDataRepository();
		var fileTypeDetector = new FileTypeDetectorService(jsonRepo, xmlRepo, archiveRepo);
		var kxService = new JsonService(jsonRepo);

		return new GridEditorService(fileTypeDetector, xmlRepo, jsonRepo, kxService);
	}
}