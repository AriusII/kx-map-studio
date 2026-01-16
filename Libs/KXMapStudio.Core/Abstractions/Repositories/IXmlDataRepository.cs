namespace KXMapStudio.Core.Abstractions.Repositories;

public interface IXmlDataRepository
{
	Task<XDocument> LoadAsync(Stream stream, CancellationToken cancellationToken = default);
	Task<XDocument> LoadFromFileAsync(string path, CancellationToken cancellationToken = default);
}