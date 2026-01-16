namespace KXMapStudio.Libs.Services;

public sealed class FileTypeDetectorService : IFileTypeDetectorService
{
	public async Task<GridSourceKind> DetectAsync(string path, CancellationToken cancellationToken = default)
	{
		var extension = Path.GetExtension(path).ToLowerInvariant();

		switch (extension)
		{
			case ".xml":
				return GridSourceKind.Xml;
			case ".taco":
				return GridSourceKind.TacoArchive;
		}

		if (extension != ".json") return GridSourceKind.Unknown;

		await using var stream = File.OpenRead(path);
		using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

		var root = doc.RootElement;

		// KX v1 (Name + Coordinates)
		if (root.TryGetProperty("Name", out _) && root.TryGetProperty("Coordinates", out _))
			return GridSourceKind.KxJson;

		// GW2 ContinentFloor (regions)
		if (root.TryGetProperty("regions", out _))
			return GridSourceKind.GuildWarsJson;

		// GW2 Maps array
		if (root.ValueKind != JsonValueKind.Array || root.GetArrayLength() <= 0) return GridSourceKind.Unknown;
		var first = root[0];
		return first.TryGetProperty("continent_id", out _)
			? GridSourceKind.GuildWarsJson
			: GridSourceKind.Unknown;
	}
}