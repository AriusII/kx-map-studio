namespace KXMapStudio.Core.Abstractions.Repositories.Serializations;

public interface IJsonDataRepository
{
	Task SaveAsync<T>(T data, string outputFile, CancellationToken cancellationToken = default);
	Task<T?> LoadAsync<T>(string path, CancellationToken cancellationToken = default);
}