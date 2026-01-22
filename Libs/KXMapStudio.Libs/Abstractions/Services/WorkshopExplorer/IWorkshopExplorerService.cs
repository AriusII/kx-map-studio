namespace KXMapStudio.Libs.Abstractions.Services.WorkshopExplorer;

public interface IWorkshopExplorerService
{
	string DataFolder { get; }
	bool IsRelevantChange(string fullPath);
	bool IsAllowedFilePath(string fullPath);

	Task<WorkshopExplorerScanNode> ScanDirectoryAsync(string directoryPath,
		CancellationToken cancellationToken = default);
}