namespace KXMapStudio.Core.Abstractions.Services.Serializations;

public interface IXmlService
{
	Task<TacoMarkerPackModel?> LoadFromFileAsync(string filePath, CancellationToken cancellationToken = default);
	Task SaveToFileAsync(TacoMarkerPackModel model, string filePath, CancellationToken cancellationToken = default);
	Task<TacoMarkerPackModel?> LoadFromStreamAsync(Stream stream, CancellationToken cancellationToken = default);
	Task SaveToStreamAsync(TacoMarkerPackModel model, Stream stream, CancellationToken cancellationToken = default);
	TacoMarkerPackModel ParseTacoMarkerPack(XDocument document);
}