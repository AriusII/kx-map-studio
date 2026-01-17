namespace KXMapStudio.Core.Abstractions.Services.Serializations;

public interface IJsonService
{
	Task<JsonModel> LoadAsync(string path, CancellationToken cancellationToken = default);
	Task SaveAsync(JsonModel data, string path, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<MapModel>> LoadGuildWarsMapsAsync(string path, CancellationToken cancellationToken = default);

	Task<ContinentFloorModel?> LoadGuildWarsContinentFloorAsync(string path,
		CancellationToken cancellationToken = default);
}