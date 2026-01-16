namespace KXMapStudio.Core.Services.Http;

public sealed record GithubHttpClient(HttpClient HttpClient) : IGithubHttpClient
{
	public async Task<bool> CurrentVersionCheck(CancellationToken cancellationToken = default)
	{
		try
		{
			var latestRelease = await HttpClient
				.GetFromJsonAsync<GitHubRelease>(Constants.Settings.GitHubApiUrl, cancellationToken);
			if (latestRelease == null || string.IsNullOrEmpty(latestRelease.TagName)) return false;

			var latestVersionString = latestRelease.TagName.TrimStart('v');
			if (!Version.TryParse(latestVersionString, out var latestVersion)) return false;

			var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
			if (currentVersion == null) return false;
			return latestVersion > currentVersion;
		}
		catch (Exception)
		{
			return false;
		}
	}
}