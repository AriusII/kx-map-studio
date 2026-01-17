namespace KXMapStudio.Libs.Services;

public sealed class FilePreviewService(
	IFileTypeDetectorService fileTypeDetectorService,
	IXmlService xmlService,
	IArchiveService archiveService,
	IArchiveDataRepository archiveDataRepository,
	IJsonService jsonService) : IFilePreviewService
{
	public async Task<FilePreviewTreeResultModel> BuildPreviewTreeAsync(string path,
		CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(path);

		var stopwatch = Stopwatch.StartNew();
		var dataType = await fileTypeDetectorService.DetectAsync(path, cancellationToken);

		var fileName = Path.GetFileName(path);
		var rootNode = new FilePreviewTreeNodeModel(fileName, FilePreviewNodeType.Root, path);

		switch (dataType)
		{
			case DataType.Xml:
				await BuildXmlTreeAsync(rootNode, path, cancellationToken);
				break;
			case DataType.TacoArchive:
				await BuildTacoArchiveTreeAsync(rootNode, path, cancellationToken);
				break;
			case DataType.KxV1Json:
				await BuildKxJsonTreeAsync(rootNode, path, cancellationToken);
				break;
			case DataType.GuildWarsJson:
				await BuildGuildWarsJsonTreeAsync(rootNode, path, cancellationToken);
				break;
		}

		stopwatch.Stop();
		return new FilePreviewTreeResultModel(path, dataType, stopwatch.Elapsed, rootNode);
	}

	private async Task BuildTacoArchiveTreeAsync(FilePreviewTreeNodeModel rootNode, string path, CancellationToken ct)
	{
		var bytes = await File.ReadAllBytesAsync(path, ct);
		var entries = await archiveDataRepository.ListContentsAsync(bytes, ct);

		// Group entries by directory structure
		var fileEntries = entries.Where(e => !e.EndsWith("/", StringComparison.Ordinal)).ToList();
		var xmlFiles = fileEntries.Where(e => e.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)).ToList();
		var otherFiles = fileEntries.Where(e => !e.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)).ToList();

		// Add folders and non-XML files
		var directoryNodes = new Dictionary<string, FilePreviewTreeNodeModel>(StringComparer.OrdinalIgnoreCase);

		foreach (var file in otherFiles)
		{
			var parts = file.Split('/', StringSplitOptions.RemoveEmptyEntries);
			var parentNode = rootNode;

			for (var i = 0; i < parts.Length; i++)
			{
				var part = parts[i];
				var isLastPart = i == parts.Length - 1;

				if (isLastPart)
				{
					// It's a file
					var fileNode = new FilePreviewTreeNodeModel(part, FilePreviewNodeType.ArchiveFile, file);
					parentNode.Children.Add(fileNode);
				}
				else
				{
					// It's a directory
					var dirPath = string.Join("/", parts.Take(i + 1));
					if (!directoryNodes.TryGetValue(dirPath, out var dirNode))
					{
						dirNode = new FilePreviewTreeNodeModel(part, FilePreviewNodeType.ArchiveFolder, dirPath);
						parentNode.Children.Add(dirNode);
						directoryNodes[dirPath] = dirNode;
					}

					parentNode = dirNode;
				}
			}
		}

		// Add XML files with their content structure
		foreach (var xmlFile in xmlFiles)
		{
			var doc = await archiveService.LoadXmlFileAsync(bytes, xmlFile, ct);
			if (doc is null) continue;

			var pack = archiveService.ParseMarkerPack(doc);
			var xmlNode =
				new FilePreviewTreeNodeModel(Path.GetFileName(xmlFile), FilePreviewNodeType.ArchiveFile, xmlFile);

			BuildTacoCategoryNodes(xmlNode, pack, xmlFile);
			rootNode.Children.Add(xmlNode);
		}
	}

	private async Task BuildXmlTreeAsync(FilePreviewTreeNodeModel rootNode, string path, CancellationToken ct)
	{
		var pack = await xmlService.LoadTacoMarkerPackAsync(path, ct);
		if (pack is null) return;

		BuildTacoCategoryNodes(rootNode, pack, path);
	}

	private static void BuildTacoCategoryNodes(FilePreviewTreeNodeModel parentNode, TacoMarkerPackModel pack,
		string basePath)
	{
		var byType = pack.Pois
			.GroupBy(p => p.Type)
			.OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase);

		foreach (var group in byType)
		{
			var categoryNode = new FilePreviewTreeNodeModel(
				group.Key,
				FilePreviewNodeType.Category,
				$"{basePath}|{group.Key}")
			{
				Count = group.Count()
			};
			parentNode.Children.Add(categoryNode);
		}

		if (pack.Trails.Count <= 0) return;
		var trailsNode = new FilePreviewTreeNodeModel(
			PreviewCategoryNames.Trails,
			FilePreviewNodeType.Category,
			$"{basePath}|{PreviewCategoryNames.Trails}")
		{
			Count = pack.Trails.Count
		};
		parentNode.Children.Add(trailsNode);
	}

	private async Task BuildKxJsonTreeAsync(FilePreviewTreeNodeModel rootNode, string path, CancellationToken ct)
	{
		var model = await jsonService.LoadKxJsonV1Async(path, ct);
		if (model is null) return;

		var coordsNode = new FilePreviewTreeNodeModel(
			PreviewCategoryNames.Coordinates,
			FilePreviewNodeType.Category,
			$"{path}|{PreviewCategoryNames.Coordinates}")
		{
			Count = model.Coordinates.Length
		};
		rootNode.Children.Add(coordsNode);
	}

	private async Task BuildGuildWarsJsonTreeAsync(FilePreviewTreeNodeModel rootNode, string path, CancellationToken ct)
	{
		await using var stream = File.OpenRead(path);
		using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);

		if (doc.RootElement.ValueKind == JsonValueKind.Array)
		{
			var maps = await jsonService.LoadGuildWarsMapsAsync(path, ct);
			var mapsNode = new FilePreviewTreeNodeModel(
				PreviewCategoryNames.Maps,
				FilePreviewNodeType.Category,
				$"{path}|{PreviewCategoryNames.Maps}")
			{
				Count = maps.Count
			};
			rootNode.Children.Add(mapsNode);
			return;
		}

		var floor = await jsonService.LoadGuildWarsContinentFloorAsync(path, ct);
		if (floor is null) return;

		BuildGw2FloorCategoryNodes(rootNode, floor, path);
	}

	private static void BuildGw2FloorCategoryNodes(FilePreviewTreeNodeModel parentNode, ContinentFloorModel floor,
		string path)
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

		if (poi > 0)
		{
			var poiNode = new FilePreviewTreeNodeModel(
				PreviewCategoryNames.PointsOfInterest,
				FilePreviewNodeType.Category,
				$"{path}|{PreviewCategoryNames.PointsOfInterest}")
			{
				Count = poi
			};
			parentNode.Children.Add(poiNode);
		}

		if (tasks > 0)
		{
			var tasksNode = new FilePreviewTreeNodeModel(
				PreviewCategoryNames.Tasks,
				FilePreviewNodeType.Category,
				$"{path}|{PreviewCategoryNames.Tasks}")
			{
				Count = tasks
			};
			parentNode.Children.Add(tasksNode);
		}

		if (sectors > 0)
		{
			var sectorsNode = new FilePreviewTreeNodeModel(
				PreviewCategoryNames.Sectors,
				FilePreviewNodeType.Category,
				$"{path}|{PreviewCategoryNames.Sectors}")
			{
				Count = sectors
			};
			parentNode.Children.Add(sectorsNode);
		}

		if (skills > 0)
		{
			var skillsNode = new FilePreviewTreeNodeModel(
				PreviewCategoryNames.SkillChallenges,
				FilePreviewNodeType.Category,
				$"{path}|{PreviewCategoryNames.SkillChallenges}")
			{
				Count = skills
			};
			parentNode.Children.Add(skillsNode);
		}

		if (masteries <= 0) return;
		var masteriesNode = new FilePreviewTreeNodeModel(
			PreviewCategoryNames.MasteryPoints,
			FilePreviewNodeType.Category,
			$"{path}|{PreviewCategoryNames.MasteryPoints}")
		{
			Count = masteries
		};
		parentNode.Children.Add(masteriesNode);
	}
}