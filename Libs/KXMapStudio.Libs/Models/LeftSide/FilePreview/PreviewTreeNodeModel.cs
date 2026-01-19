namespace KXMapStudio.Libs.Models.LeftSide.FilePreview;

public sealed partial class PreviewTreeNodeModel(
	string name,
	string? fullPath = null,
	int? count = null,
	bool isLeaf = false,
	string? archivePath = null,
	string? archiveEntryFullName = null)
	: ObservableObject
{
	[ObservableProperty] private bool _isExpanded;
	[ObservableProperty] private bool _isSelected;
	public string Name { get; } = name;
	public string? FullPath { get; } = fullPath;
	public int? Count { get; } = count;
	public bool IsLeaf { get; } = isLeaf;

	/// <summary>
	/// When set, indicates this node represents an entry inside an archive.
	/// </summary>
	public string? ArchivePath { get; } = archivePath;

	/// <summary>
	/// The full name of the archive entry (ZIP path using '/').
	/// </summary>
	public string? ArchiveEntryFullName { get; } = archiveEntryFullName;

	public bool IsArchiveEntryLeaf => IsLeaf && !string.IsNullOrWhiteSpace(ArchivePath) &&
	                                 !string.IsNullOrWhiteSpace(ArchiveEntryFullName);


	public ObservableCollection<PreviewTreeNodeModel> Children { get; } = [];
}