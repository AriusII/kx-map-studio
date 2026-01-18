namespace KXMapStudio.Core.Models.IOptions;

/// <summary>
///     Represents application settings bound from configuration.
/// </summary>
public sealed class SettingsOption
{
	/// <summary>
	///     Gets the base path used to store KX data files.
	/// </summary>
	public required string KxDataPath { get; init; }

	/// <summary>
	///     Gets a value indicating whether remote data should be pulled on startup.
	/// </summary>
	public required bool DataPullAtStartup { get; init; }

	/// <summary>
	///     Gets the hotkeys configuration.
	/// </summary>
	public required HotkeysOption HotkeysOption { get; init; }
}