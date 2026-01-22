namespace KXMapStudio.Libs.Services.WorkshopExplorer;

/// <summary>
///     UI-facing explorer service (WPF). Delegates filesystem/tree construction to Core and maps to WPF models.
/// </summary>
public sealed class WorkshopExplorerService : IWorkshopExplorerService
{
	private readonly IWorkshopExplorerNodeService _nodeService;
	private readonly IWorkshopExplorerScanner _scanner;

	public WorkshopExplorerService(
		IWorkshopExplorerNodeService nodeService,
		IWorkshopExplorerScanner scanner)
	{
		_nodeService = nodeService ?? throw new ArgumentNullException(nameof(nodeService));
		_scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));

		DataFolder = Path.Combine(AppContext.BaseDirectory, Constants.Settings.DataFolder);
		Directory.CreateDirectory(DataFolder);
	}

	public string DataFolder { get; }

	public WorkspaceExplorerNodeModel BuildRootNode(bool recursive = true)
	{
		// Kept for backward compatibility with existing synchronous VM code.
		// Internally we run the scan synchronously (no cancellation on this legacy path).
		var scan = _scanner
			.ScanAsync(DataFolder, WorkshopExplorerConstants.AllowedFileExtensions, CancellationToken.None)
			.ConfigureAwait(false)
			.GetAwaiter()
			.GetResult();

		return _nodeService.MapScanNodeToUiNode(scan);
	}

	public WorkspaceExplorerNodeModel? FindNodeByPath(WorkspaceExplorerNodeModel nodeModel, string fullPath)
	{
		return _nodeService.FindByPath(nodeModel, fullPath);
	}

	public bool IsRelevantChange(string fullPath)
	{
		if (string.IsNullOrWhiteSpace(fullPath))
			return false;

		try
		{
			var normalized = WorkshopExplorerNodeService.NormalizeFullPath(fullPath);
			var dataRoot = WorkshopExplorerNodeService.NormalizeFullPath(DataFolder);

			return WorkshopExplorerNodeService.PathStartsWith(normalized, dataRoot);
		}
		catch
		{
			return false;
		}
	}

	public bool IsAllowedFilePath(string fullPath)
	{
		if (string.IsNullOrWhiteSpace(fullPath))
			return false;

		var ext = Path.GetExtension(fullPath);
		return WorkshopExplorerConstants.AllowedFileExtensions.Contains(ext, WorkshopExplorerConstants.PathComparer);
	}
}