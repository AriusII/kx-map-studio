namespace KXMapStudio.Libs.Models.Workspace;

public sealed record FilePreviewResultModel(
	string Path,
	FileType FileType,
	TimeSpan LoadTime,
	IReadOnlyList<FilePreviewCategoryModel> Categories);