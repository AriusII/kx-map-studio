namespace KXMapStudio.Core.Models.IOptions;

/// <summary>
///     Represents hotkey settings bound from configuration.
/// </summary>
public sealed class HotkeysOption
{
	/// <summary>
	///     Gets the key used to add a marker.
	/// </summary>
	public required string AddMarkerKey { get; init; }

	/// <summary>
	///     Gets the modifier keys used to add a marker.
	/// </summary>
	public required string AddMarkerModifiers { get; init; }

	/// <summary>
	///     Gets the key used to undo the most recent marker addition.
	/// </summary>
	public required string UndoLastAddKey { get; init; }

	/// <summary>
	///     Gets the modifier keys used to undo the most recent marker addition.
	/// </summary>
	public required string UndoLastAddModifiers { get; init; }
}