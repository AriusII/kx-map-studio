namespace KXMapStudio.Libs.Abstractions.Services.StateManagement;

/// <summary>
///     Maintains an in-memory undo/redo history for an arbitrary state object.
/// </summary>
/// <typeparam name="TState">Snapshot type to store (should be treated as immutable by callers).</typeparam>
public interface IStateManagementService<TState>
{
	/// <summary>
	///     Maximum number of undo snapshots retained.
	/// </summary>
	int Capacity { get; }

	bool CanUndo { get; }
	bool CanRedo { get; }

	/// <summary>
	///     Clears undo/redo history.
	/// </summary>
	void Reset();

	/// <summary>
	///     Pushes a snapshot representing the state <em>before</em> a modification.
	///     This clears the redo stack.
	/// </summary>
	void PushSnapshot(TState snapshot);

	/// <summary>
	///     Produces the state to apply for an Undo operation.
	/// </summary>
	/// <param name="current">Current state snapshot.</param>
	TState Undo(TState current);

	/// <summary>
	///     Produces the state to apply for a Redo operation.
	/// </summary>
	/// <param name="current">Current state snapshot.</param>
	TState Redo(TState current);

	event EventHandler? StateChanged;
}