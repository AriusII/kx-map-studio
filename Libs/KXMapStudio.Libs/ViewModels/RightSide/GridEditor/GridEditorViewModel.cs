namespace KXMapStudio.Libs.ViewModels.RightSide.GridEditor;

public sealed partial class GridEditorViewModel : ObservableObject, IGridEditorViewModel
{
	private readonly IGridEditorDocumentService _documentService;
	private readonly IStateManagementService<IReadOnlyList<GridEditorRowViewModel>> _state;

	private CancellationTokenSource? _cts;
	private EditorDocumentReference? _currentDoc;

	[ObservableProperty] private string? _documentTitle;
	[ObservableProperty] private bool _isDirty;
	[ObservableProperty] private bool _isLoaded;
	private int _suppressDirty;

	public GridEditorViewModel(
		IGridEditorDocumentService documentService,
		IStateManagementService<IReadOnlyList<GridEditorRowViewModel>> state)
	{
		_documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
		_state = state ?? throw new ArgumentNullException(nameof(state));

		Rows = [];

		SaveCommand = new AsyncRelayCommand(SaveAsync, () => CanSave);
		SaveAsCommand = new AsyncRelayCommand(SaveAsAsync, () => CanSaveAs);
		UndoCommand = new RelayCommand(Undo, () => CanUndo);
		RedoCommand = new RelayCommand(Redo, () => CanRedo);
		MoveUpCommand = new RelayCommand<GridEditorRowViewModel?>(MoveUp, CanMoveUp);
		MoveDownCommand = new RelayCommand<GridEditorRowViewModel?>(MoveDown, CanMoveDown);
		AddRowCommand = new RelayCommand(AddRow, () => IsLoaded);

		_state.StateChanged += StateOnStateChanged;
	}

	public ObservableCollection<GridEditorRowViewModel> Rows { get; }

	public bool CanUndo => _state.CanUndo;
	public bool CanRedo => _state.CanRedo;

	public bool CanSave => IsLoaded && IsDirty && _currentDoc is { IsWorkspaceFile: true }
	                       && (string.Equals(_currentDoc.Extension, ".xml", StringComparison.OrdinalIgnoreCase)
	                           || string.Equals(_currentDoc.Extension, ".json", StringComparison.OrdinalIgnoreCase));

	public bool CanSaveAs => IsLoaded;

	public IAsyncRelayCommand SaveCommand { get; }
	public IAsyncRelayCommand SaveAsCommand { get; }
	public IRelayCommand UndoCommand { get; }
	public IRelayCommand RedoCommand { get; }
	public IRelayCommand<GridEditorRowViewModel?> MoveUpCommand { get; }
	public IRelayCommand<GridEditorRowViewModel?> MoveDownCommand { get; }
	public IRelayCommand AddRowCommand { get; }

	public async Task LoadAsync(EditorDocumentReference doc, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(doc);

		_cts?.Cancel();
		_cts?.Dispose();
		_cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

		// Reset history when switching documents.
		_state.Reset();

		_currentDoc = doc;
		DocumentTitle = doc.IsArchiveEntry
			? $"{doc.DisplayName} (in {Path.GetFileName(doc.ArchivePath)}) [Read-Only]"
			: doc.DisplayName;

		IsLoaded = true;

		SetDirty(false);
		NotifyCommandStateChanged();

		var ct = _cts.Token;
		var (rows, _) = await _documentService.LoadAsync(doc, ct);
		ct.ThrowIfCancellationRequested();

		ReloadRows(rows);

		SetDirty(false);
		NotifyCommandStateChanged();
	}

	public void Dispose()
	{
		_state.StateChanged -= StateOnStateChanged;

		_cts?.Cancel();
		_cts?.Dispose();
		_cts = null;

		foreach (var row in Rows)
			row.PropertyChanged -= RowOnPropertyChanged;
	}

	private void StateOnStateChanged(object? sender, EventArgs e)
	{
		NotifyCommandStateChanged();
	}

	private void ReloadRows(IReadOnlyList<GridEditorRowViewModel> rows)
	{
		Interlocked.Exchange(ref _suppressDirty, 1);
		try
		{
			Rows.Clear();

			var id = 1;
			foreach (var r in rows)
			{
				r.Id = id++;
				HookRow(r);
				Rows.Add(r);
			}
		}
		finally
		{
			Interlocked.Exchange(ref _suppressDirty, 0);
		}
	}

	private void HookRow(GridEditorRowViewModel row)
	{
		row.PropertyChanged -= RowOnPropertyChanged;
		row.PropertyChanged += RowOnPropertyChanged;
	}

	private void RowOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (Interlocked.CompareExchange(ref _suppressDirty, 0, 0) == 1)
			return;

		if (sender is not GridEditorRowViewModel)
			return;

		if (e.PropertyName is nameof(GridEditorRowViewModel.Id))
			return;

		SetDirty(true);
	}

	private void PushUndoSnapshot()
	{
		// Snapshot BEFORE a change.
		_state.PushSnapshot(SnapshotRows());
	}

	private IReadOnlyList<GridEditorRowViewModel> SnapshotRows()
	{
		return Rows.Select(CloneRow).ToList();
	}

	private static GridEditorRowViewModel CloneRow(GridEditorRowViewModel r)
	{
		return new GridEditorRowViewModel
		{
			Id = r.Id,
			Name = r.Name,
			X = r.X,
			Y = r.Y,
			Z = r.Z
		};
	}

	private void Undo()
	{
		if (!CanUndo)
			return;

		var current = SnapshotRows();
		var state = _state.Undo(current);
		ReloadRows(state);
		SetDirty(true);
	}

	private void Redo()
	{
		if (!CanRedo)
			return;

		var current = SnapshotRows();
		var state = _state.Redo(current);
		ReloadRows(state);
		SetDirty(true);
	}

	private async Task SaveAsync()
	{
		if (_currentDoc is null)
			return;

		if (!CanSave)
			return;

		// Important: do NOT push an undo snapshot for Save. State history is about edits.
		await _documentService.SaveAsync(_currentDoc, Rows.ToList(), _cts?.Token ?? CancellationToken.None);
		SetDirty(false);
		NotifyCommandStateChanged();
	}

	private async Task SaveAsAsync()
	{
		if (_currentDoc is null)
			return;

		await _documentService.SaveAsAsync(_currentDoc, Rows.ToList(), _cts?.Token ?? CancellationToken.None);
		NotifyCommandStateChanged();
	}

	private void SetDirty(bool value)
	{
		IsDirty = value;
		NotifyCommandStateChanged();
	}

	private void NotifyCommandStateChanged()
	{
		SaveCommand.NotifyCanExecuteChanged();
		SaveAsCommand.NotifyCanExecuteChanged();
		UndoCommand.NotifyCanExecuteChanged();
		RedoCommand.NotifyCanExecuteChanged();
		MoveUpCommand.NotifyCanExecuteChanged();
		MoveDownCommand.NotifyCanExecuteChanged();
		AddRowCommand.NotifyCanExecuteChanged();

		OnPropertyChanged(nameof(CanSave));
		OnPropertyChanged(nameof(CanSaveAs));
		OnPropertyChanged(nameof(CanUndo));
		OnPropertyChanged(nameof(CanRedo));
	}

	private bool CanMoveUp(GridEditorRowViewModel? row)
	{
		return row != null && Rows.IndexOf(row) > 0;
	}

	private bool CanMoveDown(GridEditorRowViewModel? row)
	{
		if (row is null)
			return false;

		var index = Rows.IndexOf(row);
		return index >= 0 && index < Rows.Count - 1;
	}

	private void MoveUp(GridEditorRowViewModel? row)
	{
		if (row is null)
			return;

		var index = Rows.IndexOf(row);
		if (index <= 0)
			return;

		PushUndoSnapshot();
		Rows.Move(index, index - 1);
		ReindexIds();
		SetDirty(true);
		MoveUpCommand.NotifyCanExecuteChanged();
		MoveDownCommand.NotifyCanExecuteChanged();
	}

	private void MoveDown(GridEditorRowViewModel? row)
	{
		if (row is null)
			return;

		var index = Rows.IndexOf(row);
		if (index < 0 || index >= Rows.Count - 1)
			return;

		PushUndoSnapshot();
		Rows.Move(index, index + 1);
		ReindexIds();
		SetDirty(true);
		MoveUpCommand.NotifyCanExecuteChanged();
		MoveDownCommand.NotifyCanExecuteChanged();
	}

	private void AddRow()
	{
		if (!IsLoaded)
			return;

		PushUndoSnapshot();

		Interlocked.Exchange(ref _suppressDirty, 1);
		try
		{
			var row = new GridEditorRowViewModel
			{
				Id = Rows.Count + 1,
				Name = string.Empty,
				X = 0d,
				Y = 0d,
				Z = 0d
			};
			HookRow(row);
			Rows.Add(row);
		}
		finally
		{
			Interlocked.Exchange(ref _suppressDirty, 0);
		}

		SetDirty(true);
	}

	private void ReindexIds()
	{
		Interlocked.Exchange(ref _suppressDirty, 1);
		try
		{
			for (var i = 0; i < Rows.Count; i++)
				Rows[i].Id = i + 1;
		}
		finally
		{
			Interlocked.Exchange(ref _suppressDirty, 0);
		}
	}
}