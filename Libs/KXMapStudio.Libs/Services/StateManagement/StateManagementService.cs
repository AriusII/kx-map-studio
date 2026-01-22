namespace KXMapStudio.Libs.Services.StateManagement;

/// <summary>
///     Default implementation of <see cref="IStateManagementService{TState}" />.
/// </summary>
public sealed class StateManagementService<TState> : IStateManagementService<TState>
{
	private readonly Stack<TState> _redo;
	private readonly Stack<TState> _undo;

	public StateManagementService(int capacity = 20)
	{
		Capacity = capacity <= 0 ? 20 : capacity;
		_undo = new Stack<TState>(Capacity);
		_redo = new Stack<TState>(Capacity);
	}

	public int Capacity { get; }

	public bool CanUndo => _undo.Count > 0;
	public bool CanRedo => _redo.Count > 0;

	public event EventHandler? StateChanged;

	public void Reset()
	{
		_undo.Clear();
		_redo.Clear();
		StateChanged?.Invoke(this, EventArgs.Empty);
	}

	public void PushSnapshot(TState snapshot)
	{
		_undo.Push(snapshot);
		_redo.Clear();
		TrimUndoToCapacity();
		StateChanged?.Invoke(this, EventArgs.Empty);
	}

	public TState Undo(TState current)
	{
		if (!CanUndo)
			return current;

		_redo.Push(current);
		var previous = _undo.Pop();
		StateChanged?.Invoke(this, EventArgs.Empty);
		return previous;
	}

	public TState Redo(TState current)
	{
		if (!CanRedo)
			return current;

		_undo.Push(current);
		TrimUndoToCapacity();

		var next = _redo.Pop();
		StateChanged?.Invoke(this, EventArgs.Empty);
		return next;
	}

	private void TrimUndoToCapacity()
	{
		if (_undo.Count <= Capacity)
			return;

		// Stack doesn't support trimming from the bottom efficiently.
		// Given the small capacity (20 by default), rebuild is fine.
		var items = _undo.Reverse().ToArray(); // bottom -> top
		var kept = items.Skip(Math.Max(0, items.Length - Capacity)).ToArray();
		_undo.Clear();
		foreach (var item in kept)
			_undo.Push(item);
	}
}