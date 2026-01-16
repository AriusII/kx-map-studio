namespace KXMapStudio.Core.Models.IOptions;

public sealed class SettingsOption
{
    public string KxDataPath { get; init; }
    public bool DataPullAtStartup { get; init; }
    public HotkeysOption HotkeysOption { get; init; }
}