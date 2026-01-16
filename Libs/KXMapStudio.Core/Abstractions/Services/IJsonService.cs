using KXMapStudio.Core.Models.Json.Kx.v1;
using KXMapStudio.Core.Types.Enums;

namespace KXMapStudio.Core.Abstractions.Services;

public interface IJsonService
{
	Task<DataType> DetectJsonTypeAsync(string path, CancellationToken cancellationToken = default);

	Task<KxModel?> LoadKxJsonV1Async(string path, CancellationToken cancellationToken = default);

	Task<IReadOnlyList<MapModel>> LoadGuildWarsMapsAsync(string path, CancellationToken cancellationToken = default);

	Task<ContinentFloorModel?> LoadGuildWarsContinentFloorAsync(string path,
		CancellationToken cancellationToken = default);

	Task SaveKxJsonV1Async(KxModel data, string path, CancellationToken cancellationToken = default);

	Task SaveGuildWarsMapsAsync(IEnumerable<MapModel> maps, string path, CancellationToken cancellationToken = default);

	Task SaveGuildWarsContinentFloorAsync(ContinentFloorModel model, string path,
		CancellationToken cancellationToken = default);
}