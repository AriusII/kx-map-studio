namespace KXMapStudio.Core.Facades;

public sealed class FileIoFacade(
	IXmlService xmlService,
	IJsonService jsonService)
	: IFileIoFacade
{
	public async Task<MarkerModel?> ReadMarkerPackAsync(
		string filePath,
		CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
		var extension = Path.GetExtension(filePath).ToLowerInvariant();
		return extension switch
		{
			FileExtension.Xml => await xmlService.LoadFromFileAsync(filePath, cancellationToken),
			_ => throw new NotSupportedException($"Extension '{extension}' non supportée.")
		};
	}

	public Task<MarkerModel?> ReadMarkerFromArchiveEntryAsync(
		string archivePath,
		string entryFullName,
		CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(archivePath);
		ArgumentException.ThrowIfNullOrWhiteSpace(entryFullName);
		throw new NotImplementedException();
	}

	public Task SaveMarkerAsync(
		MarkerModel model,
		string filePath,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(model);
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
		return xmlService.SaveToFileAsync(model, filePath, cancellationToken);
	}

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

	public Task<IReadOnlyList<string>> ListArchiveEntriesAsync(
		string archivePath,
		CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(archivePath);
		throw new NotImplementedException();
	}
}