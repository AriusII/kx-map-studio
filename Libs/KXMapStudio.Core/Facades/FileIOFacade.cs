namespace KXMapStudio.Core.Facades;

public sealed class FileIoFacade(
	IJsonService jsonService)
	: IFileIoFacade
{
	public Task<IReadOnlyList<MapModel>> ReadGw2MapsAsync(
		string filePath,
		CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
		return jsonService.LoadGuildWarsMapsAsync(filePath, cancellationToken);
	}

	public Task<ContinentFloorModel?> ReadGw2ContinentFloorAsync(
		string filePath,
		CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
		return jsonService.LoadGuildWarsContinentFloorAsync(filePath, cancellationToken);
	}

	public Task<JsonModel> ReadJsonAsync(string filePath, CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
		return jsonService.LoadAsync(filePath, cancellationToken);
	}
}