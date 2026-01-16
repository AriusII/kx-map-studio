using System.Diagnostics;
using KXMapStudio.Core.Abstractions.Services;
using KXMapStudio.Core.Models.Json.Continents;
using KXMapStudio.Core.Models.Xml.Taco;

namespace KXMapStudio.Libs.Services;

public sealed class FilePreviewService(
	IFileTypeDetectorService fileTypeDetectorService,
	IXmlService xmlService,
	IArchiveService archiveService,
	IJsonService jsonService) : IFilePreviewService
{
	public async Task<FilePreviewResultModel> BuildPreviewAsync(string path, CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(path);

		var stopwatch = Stopwatch.StartNew();
		var dataType = await fileTypeDetectorService.DetectAsync(path, cancellationToken);

		var categories = dataType switch
		{
			DataType.Xml => await BuildFromXmlAsync(path, cancellationToken),
			DataType.TacoArchive => await BuildFromTacoArchiveAsync(path, cancellationToken),
			DataType.KxV1Json => await BuildFromKxJsonAsync(path, cancellationToken),
			DataType.GuildWarsJson => await BuildFromGuildWarsJsonAsync(path, cancellationToken),
			_ => new List<FilePreviewCategoryModel>()
		};

		stopwatch.Stop();
		return new FilePreviewResultModel(path, dataType, stopwatch.Elapsed, categories);
	}

	private async Task<List<FilePreviewCategoryModel>> BuildFromXmlAsync(string path, CancellationToken ct)
	{
		var pack = await xmlService.LoadTacoMarkerPackAsync(path, ct);
		if (pack is null) return [];

		return BuildTacoCategories(pack);
	}

	private async Task<List<FilePreviewCategoryModel>> BuildFromTacoArchiveAsync(string path, CancellationToken ct)
	{
		var packs = await archiveService.LoadMarkerPacksAsync(path, ct);
		if (packs.Count == 0) return [];

		var allPois = packs.SelectMany(p => p.Pois).ToList();
		var allTrails = packs.SelectMany(p => p.Trails).ToList();

		return BuildTacoCategories(new TacoMarkerPackModel([], allPois, allTrails));
	}

	private static List<FilePreviewCategoryModel> BuildTacoCategories(TacoMarkerPackModel pack)
	{
		var items = new List<FilePreviewCategoryModel>();

		var byType = pack.Pois
			.GroupBy(p => p.Type)
			.OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase);

		foreach (var group in byType)
			items.Add(new FilePreviewCategoryModel(group.Key, group.Count()));

		if (pack.Trails.Count > 0)
			items.Add(new FilePreviewCategoryModel(PreviewCategoryNames.Trails, pack.Trails.Count));

		return items;
	}

	private async Task<List<FilePreviewCategoryModel>> BuildFromKxJsonAsync(string path, CancellationToken ct)
	{
		var model = await jsonService.LoadKxJsonV1Async(path, ct);
		if (model is null) return [];

		return [new FilePreviewCategoryModel(PreviewCategoryNames.Coordinates, model.Coordinates.Length)];
	}

	private async Task<List<FilePreviewCategoryModel>> BuildFromGuildWarsJsonAsync(string path, CancellationToken ct)
	{
		await using var stream = File.OpenRead(path);
		using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);

		if (doc.RootElement.ValueKind == JsonValueKind.Array)
		{
			var maps = await jsonService.LoadGuildWarsMapsAsync(path, ct);
			return [new FilePreviewCategoryModel(PreviewCategoryNames.Maps, maps.Count)];
		}

		var floor = await jsonService.LoadGuildWarsContinentFloorAsync(path, ct);
		return floor is null ? [] : BuildGw2FloorCategories(floor);
	}

	private static List<FilePreviewCategoryModel> BuildGw2FloorCategories(ContinentFloorModel floor)
	{
		var poi = 0;
		var tasks = 0;
		var sectors = 0;
		var skills = 0;
		var masteries = 0;

		foreach (var region in floor.Regions.Values)
		foreach (var map in region.Maps.Values)
		{
			poi += map.PointsOfInterest.Count;
			tasks += map.Tasks.Count;
			sectors += map.Sectors.Count;
			skills += map.SkillChallenges.Count;
			masteries += map.MasteryPoints.Count;
		}

		var items = new List<FilePreviewCategoryModel>();
		if (poi > 0) items.Add(new FilePreviewCategoryModel(PreviewCategoryNames.PointsOfInterest, poi));
		if (tasks > 0) items.Add(new FilePreviewCategoryModel(PreviewCategoryNames.Tasks, tasks));
		if (sectors > 0) items.Add(new FilePreviewCategoryModel(PreviewCategoryNames.Sectors, sectors));
		if (skills > 0) items.Add(new FilePreviewCategoryModel(PreviewCategoryNames.SkillChallenges, skills));
		if (masteries > 0) items.Add(new FilePreviewCategoryModel(PreviewCategoryNames.MasteryPoints, masteries));

		return items;
	}
}