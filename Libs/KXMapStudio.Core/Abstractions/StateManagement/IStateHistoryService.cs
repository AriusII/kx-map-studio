namespace KXMapStudio.Core.Abstractions.StateManagement;

/// <summary>
///     Defines an undo/redo history buffer for immutable state snapshots.
/// </summary>
/// <typeparam name="TState">The state snapshot type.</typeparam>
public interface IStateHistoryService<TState>
	where TState : class
{
	/// <summary>
	///     Gets a value indicating whether an undo operation is currently possible.
	/// </summary>
	bool CanUndo { get; }

	/// <summary>
	///     Gets a value indicating whether a redo operation is currently possible.
	/// </summary>
	bool CanRedo { get; }

	/// <summary>
	///     Gets the current state snapshot, if any.
	/// </summary>
	TState? CurrentState { get; }

	/// <summary>
	///     Pushes a new state snapshot into the history.
	/// </summary>
	/// <param name="state">The state snapshot to store.</param>
	void PushState(TState state);

	/// <summary>
	///     Undoes the last change.
	/// </summary>
	/// <returns>The new current state, or <see langword="null" /> if no undo is possible.</returns>
	TState? Undo();

	/// <summary>
	///     Redoes the last undone change.
	/// </summary>
	/// <returns>The redone state, or <see langword="null" /> if no redo is possible.</returns>
	TState? Redo();

	/// <summary>
	///     Clears both undo and redo history.
	/// </summary>
	void Clear();
}