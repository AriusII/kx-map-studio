namespace KXMapStudio.Libs.Models.Workspace;

public sealed record FilePreviewResultModel(
	string Path,
	DataType DataType,
	TimeSpan LoadTime,
	IReadOnlyList<FilePreviewCategoryModel> Categories);