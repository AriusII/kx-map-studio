namespace KXMapStudio.Core.Abstractions.Services;

public interface IXmlService
{
	Task<TacoMarkerPackModel?> LoadTacoMarkerPackAsync(string path, CancellationToken cancellationToken = default);

	Task SaveTacoMarkerPackAsync(TacoMarkerPackModel model, string path, CancellationToken cancellationToken = default);
}