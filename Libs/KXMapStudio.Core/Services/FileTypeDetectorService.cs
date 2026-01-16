using KXMapStudio.Core.Types.Enums;

namespace KXMapStudio.Core.Services;

public sealed record FileTypeDetectorService(
	IJsonDataRepository JsonDataRepository,
	IXmlDataRepository XmlDataRepository,
	IArchiveDataRepository ArchiveDataRepository) : IFileTypeDetectorService
{
	public async Task<DataType> DetectAsync(string path, CancellationToken cancellationToken = default)
	{
		var extension = Path.GetExtension(path).ToLowerInvariant();

		switch (extension)
		{
			case ".xml":
				return DataType.Xml;
			case ".taco":
			case ".zip":
				return DataType.TacoArchive;
		}

		if (extension != ".json") return DataType.Unknown;

		await using var stream = File.OpenRead(path);
		using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

		var root = doc.RootElement;

		// KX v1 (Name + Coordinates)
		if (root.TryGetProperty("Name", out _) && root.TryGetProperty("Coordinates", out _))
			return DataType.KxV1Json;

		// GW2 ContinentFloor (regions)
		if (root.TryGetProperty("regions", out _))
			return DataType.GuildWarsJson;

		if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
		{
			var first = root[0];
			if (first.TryGetProperty("continent_id", out _))
				return DataType.GuildWarsJson;
		}

		if (root.TryGetProperty("version", out _) || root.TryGetProperty("markers", out _))
			return DataType.KxV2Json;

		return DataType.Unknown;
	}
}