namespace KXMapStudio.Core.Abstractions.Repositories.Serializations;

public interface IXmlDataRepository
{
	Task SaveToFileAsync(XDocument document, string path, CancellationToken cancellationToken = default);
	Task<XDocument?> LoadFromFileAsync(string path, CancellationToken cancellationToken = default);
	Task SaveToStreamAsync(XDocument document, Stream stream, CancellationToken cancellationToken = default);
	Task<XDocument?> LoadFromStreamAsync(Stream stream, CancellationToken cancellationToken = default);
}