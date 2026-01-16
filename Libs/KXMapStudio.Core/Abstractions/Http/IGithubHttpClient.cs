namespace KXMapStudio.Core.Abstractions.Http;

public interface IGithubHttpClient
{
	Task<bool> CheckCurrentVersion(CancellationToken cancellationToken = default);
}