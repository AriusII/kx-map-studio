namespace KXMapStudio.Core.Abstractions.StateManagement;

/// <summary>
///     Service for managing state history with undo/redo capabilities.
/// </summary>
/// <typeparam name="TState">The type of state to manage.</typeparam>
public interface IStateHistoryService<TState> where TState : class
{
	/// <summary>
	///     Gets whether undo operation is available.
	/// </summary>
	bool CanUndo { get; }

	/// <summary>
	///     Gets whether redo operation is available.
	/// </summary>
	bool CanRedo { get; }

	/// <summary>
	///     Gets the current state.
	/// </summary>
	TState? CurrentState { get; }

	/// <summary>
	///     Pushes a new state to the history.
	/// </summary>
	void PushState(TState state);

	/// <summary>
	///     Undoes the last state change and returns the previous state.
	/// </summary>
	TState? Undo();

	/// <summary>
	///     Redoes the last undone state change and returns the next state.
	/// </summary>
	TState? Redo();

	/// <summary>
	///     Clears all history.
	/// </summary>
	void Clear();
}