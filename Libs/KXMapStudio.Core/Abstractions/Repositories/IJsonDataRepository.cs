namespace KXMapStudio.Core.Abstractions.Repositories;

public interface IJsonDataRepository
{
	Task SaveDataAsync<T>(T data, string outputFile, CancellationToken cancellationToken = default);
	Task<T?> LoadDataAsync<T>(string path, CancellationToken cancellationToken = default);
}