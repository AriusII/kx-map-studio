namespace KXMapStudio.Libs.Models.Workspace;

/// <summary>
///     Represents a node in the file preview tree structure.
/// </summary>
public sealed partial class FilePreviewTreeNodeModel(string name, FilePreviewNodeType nodeType, string? fullPath = null)
	: ObservableObject
{
	/// <summary>
	///     Number of items in this node (for categories).
	/// </summary>
	[ObservableProperty] private int _count;

	/// <summary>
	///     Whether this node is expanded in the TreeView.
	/// </summary>
	[ObservableProperty] private bool _isExpanded;

	/// <summary>
	///     Whether this node is selected in the TreeView.
	/// </summary>
	[ObservableProperty] private bool _isSelected;

	/// <summary>
	///     Display name of the node.
	/// </summary>
	public string Name { get; } = name;

	/// <summary>
	///     Type of node (archive root, xml file, category, etc.).
	/// </summary>
	public FilePreviewNodeType NodeType { get; } = nodeType;

	/// <summary>
	///     Full path within the archive or file system (e.g., "./Data/map.xml" or "Waypoints").
	///     Used to identify the exact data to load.
	/// </summary>
	public string? FullPath { get; } = fullPath;

	/// <summary>
	///     Child nodes.
	/// </summary>
	public ObservableCollection<FilePreviewTreeNodeModel> Children { get; } = [];

	/// <summary>
	///     Display text showing name and count.
	/// </summary>
	public string DisplayText => Count > 0 ? $"{Name} ({Count})" : Name;
}