namespace KXMapStudio.Libs.Models.LeftSide.FilePreview;

public sealed record FilePreviewResult(
	string OpenedFileName,
	string PreviewStatus,
	string PreviewLoadTimeText,
	IReadOnlyList<PreviewTreeNodeModel> Nodes);