namespace KXMapStudio.Libs.Models.Workspace;

/// <summary>
///     Represents the result of building a file preview tree structure.
/// </summary>
public sealed record FilePreviewTreeResultModel(
	string Path,
	DataType DataType,
	TimeSpan LoadTime,
	FilePreviewTreeNodeModel RootNode);