namespace KXMapStudio.Core.Services.Http;

/// <summary>
///     Provides minimal GitHub API operations used by KXMapStudio.
/// </summary>
/// <param name="HttpClient">The HTTP client used to call the GitHub API.</param>
public sealed record GithubHttpClient(HttpClient HttpClient) : IGithubHttpClient
{
	/// <summary>
	///     Checks if the latest GitHub release version is newer than the currently executing assembly version.
	/// </summary>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	/// <returns>
	///     <see langword="true" /> when a newer version is available; otherwise, <see langword="false" />.
	/// </returns>
	/// <remarks>
	///     This method is intentionally defensive: any failure (network, invalid payload, missing version) returns
	///     <see langword="false" /> to avoid blocking the application startup or UI.
	/// </remarks>
	public async Task<bool> CheckCurrentVersion(CancellationToken cancellationToken = default)
	{
		try
		{
			var latestRelease = await GetLatestReleaseAsync(cancellationToken);
			if (latestRelease == null)
				return false;

			var latestVersionString = latestRelease.TagName.TrimStart('v');
			if (!Version.TryParse(latestVersionString, out var latestVersion))
				return false;

			var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
			if (currentVersion == null)
				return false;

			return latestVersion > currentVersion;
		}
		catch
		{
			return false;
		}
	}

	/// <summary>
	///     Gets the latest release information from GitHub.
	/// </summary>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	/// <returns>
	///     The latest release information, or <see langword="null"/> if the request fails or no release is available.
	/// </returns>
	/// <remarks>
	///     This method is intentionally defensive: any failure (network, invalid payload) returns
	///     <see langword="null"/> to avoid blocking the application startup or UI.
	/// </remarks>
	public async Task<GitHubReleaseModel?> GetLatestReleaseAsync(CancellationToken cancellationToken = default)
	{
		try
		{
			var latestRelease = await HttpClient.GetFromJsonAsync<GitHubReleaseModel>(
				Constants.Settings.GitHubApiUrl,
				JsonRepository.DefaultJsonOptions,
				cancellationToken);

			return latestRelease;
		}
		catch
		{
			return null;
		}
	}
}