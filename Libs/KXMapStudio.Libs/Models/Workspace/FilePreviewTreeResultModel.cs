namespace KXMapStudio.Libs.Models.Workspace;

/// <summary>
///     Represents the result of building a file preview tree structure.
/// </summary>
public sealed record FilePreviewTreeResultModel(
	string Path,
	FileType FileType,
	TimeSpan LoadTime,
	FilePreviewTreeNodeModel RootNode);