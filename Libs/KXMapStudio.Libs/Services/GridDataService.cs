using KXMapStudio.Core.Abstractions.Repositories.Serializations;
using KXMapStudio.Core.Abstractions.Services.Serializations;

namespace KXMapStudio.Libs.Services;

public sealed class GridDataService(
	IFileTypeDetectorService fileTypeDetectorService,
	IXmlDataRepository xmlDataRepository,
	IJsonDataRepository jsonDataRepository,
	IArchiveDataRepository archiveDataRepository,
	IJsonService jsonService)
	: IGridDataService
{
	public async Task<IReadOnlyList<GridRowDto>> LoadRowsAsync(string path, string nodePath,
		CancellationToken cancellationToken = default)
	{
		var kind = await fileTypeDetectorService.DetectAsync(path, cancellationToken);

		// Parse nodePath: format is "path|category" or "archivePath|xmlFile|category"
		var parts = nodePath.Split('|', StringSplitOptions.RemoveEmptyEntries);
		var category = parts.Length > 0 ? parts[^1] : string.Empty;
		var xmlFileInArchive = parts.Length > 2 ? parts[1] : null;

		return kind switch
		{
			DataType.Xml => await LoadFromXmlFileAsync(path, category, cancellationToken),
			DataType.TacoArchive => await LoadFromTacoArchiveAsync(path, category, xmlFileInArchive, cancellationToken),
			DataType.KxV1Json => await LoadFromKxJsonAsync(path, category, cancellationToken),
			DataType.GuildWarsJson => await LoadFromGuildWarsJsonAsync(path, category, cancellationToken),
			_ => []
		};
	}

	private async Task<IReadOnlyList<GridRowDto>> LoadFromXmlFileAsync(string path, string category,
		CancellationToken cancellationToken)
	{
		var doc = await xmlDataRepository.LoadFromFileAsync(path, cancellationToken);
		var pack = DeserializeTacoPack(doc);
		return MapTacoData(pack, category);
	}

	private async Task<IReadOnlyList<GridRowDto>> LoadFromTacoArchiveAsync(string path, string category,
		string? specificXmlFile, CancellationToken cancellationToken)
	{
		var bytes = await File.ReadAllBytesAsync(path, cancellationToken);
		var entries = await archiveDataRepository.ListContentsAsync(bytes, cancellationToken);

		var allPois = new List<TacoPoiModel>();
		var allTrails = new List<TacoTrailModel>();

		var xmlFiles = entries.Where(e => e.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)).ToList();

		// If a specific XML file is requested, filter to only that file
		if (!string.IsNullOrWhiteSpace(specificXmlFile))
			xmlFiles = xmlFiles.Where(x => x.Equals(specificXmlFile, StringComparison.OrdinalIgnoreCase)).ToList();

		foreach (var entry in xmlFiles)
		{
			await using var stream = await archiveDataRepository.GetEntryStreamAsync(bytes, entry, cancellationToken);
			if (stream is null) continue;

			var doc = await xmlDataRepository.LoadFromArchiveAsync(stream, cancellationToken);
			var pack = DeserializeTacoPack(doc);
			allPois.AddRange(pack.Pois);
			allTrails.AddRange(pack.Trails);
		}

		return MapTacoData(new TacoMarkerPackModel([], allPois, allTrails), category);
	}

	private async Task<IReadOnlyList<GridRowDto>> LoadFromKxJsonAsync(string path, string category,
		CancellationToken cancellationToken)
	{
		if (!string.IsNullOrWhiteSpace(category) &&
		    !string.Equals(category, PreviewCategoryNames.Coordinates, StringComparison.OrdinalIgnoreCase))
			return [];

		var model = await jsonService.LoadAsync(path, cancellationToken);
		if (model is null) return [];

		var rows = new List<GridRowDto>();
		var id = 1;

		foreach (var coord in model.Coordinates)
			rows.Add(new GridRowDto(
				id++,
				coord.Name,
				coord.X,
				coord.Y,
				coord.Z));

		return rows;
	}

	private async Task<IReadOnlyList<GridRowDto>> LoadFromGuildWarsJsonAsync(string path, string category,
		CancellationToken cancellationToken)
	{
		await using var stream = File.OpenRead(path);
		using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

		if (doc.RootElement.ValueKind == JsonValueKind.Array)
		{
			if (!string.IsNullOrWhiteSpace(category) &&
			    !string.Equals(category, PreviewCategoryNames.Maps, StringComparison.OrdinalIgnoreCase))
				return [];

			var maps = await jsonDataRepository.LoadAsync<List<MapModel>>(path, cancellationToken);
			return MapGw2Maps(maps ?? []);
		}

		var floor = await jsonDataRepository.LoadAsync<ContinentFloorModel>(path, cancellationToken);
		if (floor is null) return [];

		return category switch
		{
			_ when string.Equals(category, PreviewCategoryNames.PointsOfInterest, StringComparison.OrdinalIgnoreCase)
				=> MapGw2PointsOfInterest(floor),
			_ when string.Equals(category, PreviewCategoryNames.Tasks, StringComparison.OrdinalIgnoreCase)
				=> MapGw2Tasks(floor),
			_ when string.Equals(category, PreviewCategoryNames.Sectors, StringComparison.OrdinalIgnoreCase)
				=> MapGw2Sectors(floor),
			_ when string.Equals(category, PreviewCategoryNames.SkillChallenges, StringComparison.OrdinalIgnoreCase)
				=> MapGw2SkillChallenges(floor),
			_ when string.Equals(category, PreviewCategoryNames.MasteryPoints, StringComparison.OrdinalIgnoreCase)
				=> MapGw2MasteryPoints(floor),
			_ => []
		};
	}

	private static IReadOnlyList<GridRowDto> MapTacoData(TacoMarkerPackModel pack, string category)
	{
		if (string.Equals(category, PreviewCategoryNames.Trails, StringComparison.OrdinalIgnoreCase))
			return MapTacoTrails(pack.Trails);

		if (!string.IsNullOrWhiteSpace(category))
			return MapTacoPois(pack.Pois.Where(p => string.Equals(p.Type, category,
				StringComparison.OrdinalIgnoreCase)));

		return MapTacoPois(pack.Pois);
	}

	private static IReadOnlyList<GridRowDto> MapTacoTrails(IEnumerable<TacoTrailModel> trails)
	{
		var rows = new List<GridRowDto>();
		var id = 1;

		foreach (var trail in trails)
			rows.Add(new GridRowDto(
				id++,
				trail.Type,
				0,
				0,
				0));

		return rows;
	}

	private static IReadOnlyList<GridRowDto> MapGw2PointsOfInterest(ContinentFloorModel floor)
	{
		var rows = new List<GridRowDto>();
		var id = 1;

		foreach (var region in floor.Regions.Values)
		foreach (var map in region.Maps.Values)
		foreach (var poi in map.PointsOfInterest.Values)
			AddFromCoord(rows, ref id, poi.Name ?? poi.Type, poi.Coord);

		return rows;
	}

	private static IReadOnlyList<GridRowDto> MapGw2Tasks(ContinentFloorModel floor)
	{
		var rows = new List<GridRowDto>();
		var id = 1;

		foreach (var region in floor.Regions.Values)
		foreach (var map in region.Maps.Values)
		foreach (var task in map.Tasks.Values)
			AddFromCoord(rows, ref id, task.Objective, task.Coord);

		return rows;
	}

	private static IReadOnlyList<GridRowDto> MapGw2Sectors(ContinentFloorModel floor)
	{
		var rows = new List<GridRowDto>();
		var id = 1;

		foreach (var region in floor.Regions.Values)
		foreach (var map in region.Maps.Values)
		foreach (var sector in map.Sectors.Values)
			AddFromCoord(rows, ref id, sector.Name, sector.Coord);

		return rows;
	}

	private static IReadOnlyList<GridRowDto> MapGw2SkillChallenges(ContinentFloorModel floor)
	{
		var rows = new List<GridRowDto>();
		var id = 1;

		foreach (var region in floor.Regions.Values)
		foreach (var map in region.Maps.Values)
		foreach (var skill in map.SkillChallenges)
			AddFromCoord(rows, ref id, $"Skill {skill.Id}", skill.Coord);

		return rows;
	}

	private static IReadOnlyList<GridRowDto> MapGw2MasteryPoints(ContinentFloorModel floor)
	{
		var rows = new List<GridRowDto>();
		var id = 1;

		foreach (var region in floor.Regions.Values)
		foreach (var map in region.Maps.Values)
		foreach (var mastery in map.MasteryPoints)
			AddFromCoord(rows, ref id, mastery.Region, mastery.Coord);

		return rows;
	}

	private static TacoMarkerPackModel DeserializeTacoPack(XDocument doc)
	{
		var serializer = new XmlSerializer(typeof(TacoOverlayDataDto));
		using var reader = doc.CreateReader();
		var dto = (TacoOverlayDataDto?)serializer.Deserialize(reader) ?? new TacoOverlayDataDto();
		return TacoMapper.MapToDomain(dto);
	}

	private static IReadOnlyList<GridRowDto> MapTacoPois(IEnumerable<TacoPoiModel> pois)
	{
		var rows = new List<GridRowDto>();
		var id = 1;

		foreach (var poi in pois)
			rows.Add(new GridRowDto(
				id++,
				poi.Type,
				poi.X,
				poi.Y,
				poi.Z));

		return rows;
	}

	private static IReadOnlyList<GridRowDto> MapGw2Maps(IEnumerable<MapModel> maps)
	{
		var rows = new List<GridRowDto>();
		var id = 1;

		foreach (var map in maps)
			rows.Add(new GridRowDto(
				id++,
				map.Name,
				0,
				0,
				0));

		return rows;
	}

	private static void AddFromCoord(List<GridRowDto> rows, ref int id, string name, double[] coord)
	{
		var x = coord.Length > 0 ? coord[0] : 0;
		var y = coord.Length > 1 ? coord[1] : 0;
		var z = coord.Length > 2 ? coord[2] : 0;

		rows.Add(new GridRowDto(id++, name, x, y, z));
	}
}