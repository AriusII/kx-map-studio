namespace KXMapStudio.Core.Abstractions.StateManagement;

public interface IStateHistoryService<TState>
	where TState : class
{
	bool CanUndo { get; }
	bool CanRedo { get; }
	TState? CurrentState { get; }
	void PushState(TState state);
	TState? Undo();
	TState? Redo();
	void Clear();
}