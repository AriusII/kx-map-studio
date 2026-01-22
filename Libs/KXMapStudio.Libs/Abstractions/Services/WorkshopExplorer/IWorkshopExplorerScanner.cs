namespace KXMapStudio.Libs.Abstractions.Services.WorkshopExplorer;

/// <summary>
///     Scans a workshop folder and returns a filesystem tree of directories and allowed files.
/// </summary>
/// <remarks>
///     This service is UI-facing but WPF-agnostic (no WPF types). It is designed to be unit-testable.
/// </remarks>
public interface IWorkshopExplorerScanner
{
	/// <summary>
	///     Scans <paramref name="rootPath" /> recursively and returns the root node.
	/// </summary>
	/// <param name="rootPath">The absolute root folder to scan.</param>
	/// <param name="allowedExtensions">
	///     Allowed file extensions (e.g. <c>.xml</c>, <c>.json</c>). Comparison should be case-insensitive.
	/// </param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>The root node representing <paramref name="rootPath" />.</returns>
	Task<WorkshopExplorerScanNode> ScanAsync(
		string rootPath,
		IReadOnlyCollection<string> allowedExtensions,
		CancellationToken cancellationToken);
}