namespace KXMapStudio.Core.Abstractions.Http;

public interface IGithubHttpClient
{
	Task<bool> CurrentVersionCheck(CancellationToken cancellationToken = default);
}