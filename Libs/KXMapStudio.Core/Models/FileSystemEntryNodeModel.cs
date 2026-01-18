namespace KXMapStudio.Core.Models;

/// <summary>
///     Core tree node (UI-agnostic).
/// </summary>
public sealed class FileSystemEntryNodeModel(string name, string fullPath, ExplorerEntryType type)
{
	public string Name { get; } = name;
	public string FullPath { get; } = fullPath;
	public ExplorerEntryType Type { get; } = type;
	public List<FileSystemEntryNodeModel> Children { get; } = new();
}