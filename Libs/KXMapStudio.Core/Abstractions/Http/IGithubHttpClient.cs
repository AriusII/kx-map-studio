namespace KXMapStudio.Core.Abstractions.Http;

/// <summary>
///     Defines HTTP operations against GitHub used by the application.
/// </summary>
public interface IGithubHttpClient
{
	/// <summary>
	///     Checks whether a newer version than the current assembly is available on GitHub.
	/// </summary>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	/// <returns><see langword="true" /> when an update is available; otherwise, <see langword="false" />.</returns>
	Task<bool> CheckCurrentVersion(CancellationToken cancellationToken = default);
}