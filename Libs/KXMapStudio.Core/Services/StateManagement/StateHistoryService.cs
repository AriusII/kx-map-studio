namespace KXMapStudio.Core.Services.StateManagement;

/// <summary>
///     Implementation of state history service with configurable maximum history size.
/// </summary>
public sealed class StateHistoryService<TState> : IStateHistoryService<TState> where TState : class
{
	private const int DefaultMaxHistorySize = 20;
	private readonly int _maxHistorySize;
	private readonly Stack<TState> _redoStack = new();
	private readonly Stack<TState> _undoStack = new();

	public StateHistoryService(int maxHistorySize = DefaultMaxHistorySize)
	{
		if (maxHistorySize <= 0)
			throw new ArgumentOutOfRangeException(nameof(maxHistorySize), "Max history size must be greater than 0.");

		_maxHistorySize = maxHistorySize;
	}

	public bool CanUndo => _undoStack.Count > 0;
	public bool CanRedo => _redoStack.Count > 0;
	public TState? CurrentState => _undoStack.Count > 0 ? _undoStack.Peek() : null;

	public void PushState(TState state)
	{
		ArgumentNullException.ThrowIfNull(state);

		_undoStack.Push(state);
		_redoStack.Clear();

		// Limit history size
		if (_undoStack.Count <= _maxHistorySize) return;
		var items = _undoStack.ToList();
		_undoStack.Clear();
		for (var i = 0; i < _maxHistorySize; i++)
			_undoStack.Push(items[i]);
	}

	public TState? Undo()
	{
		if (!CanUndo)
			return null;

		var current = _undoStack.Pop();
		_redoStack.Push(current);

		return CurrentState;
	}

	public TState? Redo()
	{
		if (!CanRedo)
			return null;

		var state = _redoStack.Pop();
		_undoStack.Push(state);

		return state;
	}

	public void Clear()
	{
		_undoStack.Clear();
		_redoStack.Clear();
	}
}