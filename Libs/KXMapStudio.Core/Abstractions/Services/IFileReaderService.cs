namespace KXMapStudio.Core.Abstractions.Services;

public interface IFileReaderService
{
	Task<TacoMarkerPackModel?> ReadTacoAsync(string filePath, FileType fileType,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyList<MapModel>> ReadMapsJsonAsync(string filePath, CancellationToken cancellationToken = default);
	Task<ContinentFloorModel?> ReadContinentsJsonAsync(string filePath, CancellationToken cancellationToken = default);
	Task<JsonModel> ReadJsonAsync(string filePath, CancellationToken cancellationToken = default);
}