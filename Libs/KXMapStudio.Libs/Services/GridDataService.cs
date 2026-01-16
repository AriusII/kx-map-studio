using System.Xml.Linq;
using System.Xml.Serialization;
using KXMapStudio.Core.Abstractions.Repositories;
using KXMapStudio.Core.Abstractions.Services;
using KXMapStudio.Core.Mappers;
using KXMapStudio.Core.Models.Json.Continents;
using KXMapStudio.Core.Models.Json.Maps;
using KXMapStudio.Core.Models.Xml.Taco;
using KXMapStudio.Core.Models.Xml.Taco.Dtos;

namespace KXMapStudio.Libs.Services;

public sealed class GridDataService(
	IFileTypeDetectorService fileTypeDetectorService,
	IXmlDataRepository xmlDataRepository,
	IJsonDataRepository jsonDataRepository,
	IArchiveDataRepository archiveDataRepository,
	IJsonService jsonService)
	: IGridDataService
{
	public async Task<IReadOnlyList<GridRowDto>> LoadRowsAsync(string path,
		CancellationToken cancellationToken = default)
	{
		var kind = await fileTypeDetectorService.DetectAsync(path, cancellationToken);

		return kind switch
		{
			DataType.Xml => await LoadFromXmlFileAsync(path, cancellationToken),
			DataType.TacoArchive => await LoadFromTacoArchiveAsync(path, cancellationToken),
			DataType.KxV1Json => await LoadFromKxJsonAsync(path, cancellationToken),
			DataType.GuildWarsJson => await LoadFromGuildWarsJsonAsync(path, cancellationToken),
			_ => []
		};
	}

	private async Task<IReadOnlyList<GridRowDto>> LoadFromXmlFileAsync(string path, CancellationToken cancellationToken)
	{
		var doc = await xmlDataRepository.LoadFromFileAsync(path, cancellationToken);
		var pack = DeserializeTacoPack(doc);
		return MapTacoPois(pack.Pois);
	}

	private async Task<IReadOnlyList<GridRowDto>> LoadFromTacoArchiveAsync(string path,
		CancellationToken cancellationToken)
	{
		var bytes = await File.ReadAllBytesAsync(path, cancellationToken);
		var entries = await archiveDataRepository.ListContentsAsync(bytes, cancellationToken);

		var allRows = new List<GridRowDto>();
		foreach (var entry in entries.Where(e => e.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)))
		{
			await using var stream = await archiveDataRepository.GetEntryStreamAsync(bytes, entry, cancellationToken);
			if (stream is null) continue;

			var doc = await xmlDataRepository.LoadFromArchiveAsync(stream, cancellationToken);
			var pack = DeserializeTacoPack(doc);
			allRows.AddRange(MapTacoPois(pack.Pois));
		}

		return allRows;
	}

	private async Task<IReadOnlyList<GridRowDto>> LoadFromKxJsonAsync(string path, CancellationToken cancellationToken)
	{
		var model = await jsonService.LoadKxJsonV1Async(path, cancellationToken);
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

	private async Task<IReadOnlyList<GridRowDto>> LoadFromGuildWarsJsonAsync(string path,
		CancellationToken cancellationToken)
	{
		await using var stream = File.OpenRead(path);
		using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

		if (doc.RootElement.ValueKind == JsonValueKind.Array)
		{
			var maps = await jsonDataRepository.LoadDataAsync<List<MapModel>>(path, cancellationToken);
			return MapGw2Maps(maps ?? []);
		}

		var floor = await jsonDataRepository.LoadDataAsync<ContinentFloorModel>(path, cancellationToken);
		return floor is null ? [] : MapGw2ContinentFloor(floor);
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

	private static IReadOnlyList<GridRowDto> MapGw2ContinentFloor(ContinentFloorModel floor)
	{
		var rows = new List<GridRowDto>();
		var id = 1;

		foreach (var region in floor.Regions.Values)
		foreach (var map in region.Maps.Values)
		{
			foreach (var poi in map.PointsOfInterest.Values)
				AddFromCoord(rows, ref id, poi.Name ?? poi.Type, poi.Coord);

			foreach (var task in map.Tasks.Values) AddFromCoord(rows, ref id, task.Objective, task.Coord);

			foreach (var sector in map.Sectors.Values) AddFromCoord(rows, ref id, sector.Name, sector.Coord);

			foreach (var skill in map.SkillChallenges) AddFromCoord(rows, ref id, $"Skill {skill.Id}", skill.Coord);

			foreach (var mastery in map.MasteryPoints) AddFromCoord(rows, ref id, mastery.Region, mastery.Coord);
		}

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