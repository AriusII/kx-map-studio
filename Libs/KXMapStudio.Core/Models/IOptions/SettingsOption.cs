namespace KXMapStudio.Core.Models.IOptions;

public sealed class SettingsOption
{
	public required string KxDataPath { get; init; }
	public required bool DataPullAtStartup { get; init; }
	public required HotkeysOption HotkeysOption { get; init; }
}