namespace KXMapStudio.Libs.Services.WorkshopExplorer;

/// <summary>
///     UI-facing explorer service (WPF). Delegates filesystem/tree construction to Core and maps to WPF models.
/// </summary>
public sealed class WorkshopExplorerService : IWorkshopExplorerService
{
	private static readonly IReadOnlyCollection<string> AllowedWorkshopExtensions =
	[
		FileExtension.Xml,
		FileExtension.Json
	];

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
			.ScanAsync(DataFolder, AllowedWorkshopExtensions, CancellationToken.None)
			.ConfigureAwait(false)
			.GetAwaiter()
			.GetResult();

		return MapToUiNode(scan);
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
			var normalized = Path.GetFullPath(fullPath);
			var dataRoot = Path.GetFullPath(DataFolder);

			return normalized.StartsWith(dataRoot, StringComparison.OrdinalIgnoreCase);
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
		return AllowedWorkshopExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase);
	}

	private static WorkspaceExplorerNodeModel MapToUiNode(WorkshopExplorerScanNode scanNode)
	{
		var ui = new WorkspaceExplorerNodeModel(scanNode.Name, scanNode.FullPath, scanNode.IsDirectory);
		foreach (var child in scanNode.Children)
			ui.Children.Add(MapToUiNode(child));
		return ui;
	}
}