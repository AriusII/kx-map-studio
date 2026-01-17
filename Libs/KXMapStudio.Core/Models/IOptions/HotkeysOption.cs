namespace KXMapStudio.Core.Models.IOptions;

public sealed class HotkeysOption
{
	public required string AddMarkerKey { get; init; }
	public required string AddMarkerModifiers { get; init; }
	public required string UndoLastAddKey { get; init; }
	public required string UndoLastAddModifiers { get; init; }
}