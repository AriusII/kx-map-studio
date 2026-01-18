namespace KXMapStudio.Core.Services.StateManagement;

/// <summary>
///     Provides an undo/redo history buffer for immutable state snapshots.
/// </summary>
/// <typeparam name="TState">The state snapshot type.</typeparam>
public sealed class StateHistoryService<TState> : IStateHistoryService<TState> where TState : class
{
	private const int DefaultMaxHistorySize = 20;
	private readonly int _maxHistorySize;
	private readonly Stack<TState> _redoStack = new();
	private readonly Stack<TState> _undoStack = new();

	/// <summary>
	///     Initializes a new instance of the <see cref="StateHistoryService{TState}" /> class.
	/// </summary>
	/// <param name="maxHistorySize">The maximum number of undo snapshots to keep.</param>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="maxHistorySize" /> is less than 1.</exception>
	public StateHistoryService(int maxHistorySize = DefaultMaxHistorySize)
	{
		if (maxHistorySize <= 0)
			throw new ArgumentOutOfRangeException(nameof(maxHistorySize), "Max history size must be greater than 0.");

		_maxHistorySize = maxHistorySize;
	}

	/// <inheritdoc />
	public bool CanUndo => _undoStack.Count > 0;

	/// <inheritdoc />
	public bool CanRedo => _redoStack.Count > 0;

	/// <inheritdoc />
	public TState? CurrentState => _undoStack.Count > 0 ? _undoStack.Peek() : null;

	/// <inheritdoc />
	public void PushState(TState state)
	{
		ArgumentNullException.ThrowIfNull(state);

		_undoStack.Push(state);
		_redoStack.Clear();

		TrimUndoStackIfNeeded();
	}

	/// <inheritdoc />
	public TState? Undo()
	{
		if (!CanUndo)
			return null;

		var current = _undoStack.Pop();
		_redoStack.Push(current);

		return CurrentState;
	}

	/// <inheritdoc />
	public TState? Redo()
	{
		if (!CanRedo)
			return null;

		var state = _redoStack.Pop();
		_undoStack.Push(state);

		return state;
	}

	/// <inheritdoc />
	public void Clear()
	{
		_undoStack.Clear();
		_redoStack.Clear();
	}

	private void TrimUndoStackIfNeeded()
	{
		if (_undoStack.Count <= _maxHistorySize)
			return;

		// Stack enumerates from top to bottom. Keep the newest N snapshots (top-first).
		var keep = new List<TState>(_maxHistorySize);
		var i = 0;
		foreach (var item in _undoStack)
		{
			keep.Add(item);
			if (++i >= _maxHistorySize)
				break;
		}

		_undoStack.Clear();
		for (var j = keep.Count - 1; j >= 0; j--)
			_undoStack.Push(keep[j]);
	}
}