namespace KXMapStudio.Libs.Services;

public sealed class GridEditorService(
	IFileTypeDetectorService fileTypeDetectorService,
	IXmlDataRepository xmlDataRepository,
	IJsonDataRepository jsonDataRepository,
	IJsonService jsonService)
	: IGridEditorService
{
	public async Task SaveAsync(string filePath, string nodePath, IReadOnlyList<GridRowDto> rows,
		CancellationToken cancellationToken = default)
	{
		var kind = await fileTypeDetectorService.DetectAsync(filePath, cancellationToken);

		switch (kind)
		{
			case DataType.Xml:
				await SaveXmlFileAsync(filePath, nodePath, rows, cancellationToken);
				break;
			case DataType.KxV1Json:
				await SaveKxJsonAsync(filePath, rows, cancellationToken);
				break;
			case DataType.TacoArchive:
			case DataType.GuildWarsJson:
			default:
				throw new NotSupportedException($"Saving {kind} files is not supported yet.");
		}
	}

	private async Task SaveXmlFileAsync(string filePath, string nodePath, IReadOnlyList<GridRowDto> rows,
		CancellationToken cancellationToken)
	{
		var doc = await xmlDataRepository.LoadFromFileAsync(filePath, cancellationToken);
		var serializer = new XmlSerializer(typeof(TacoOverlayDataDto));

		TacoOverlayDataDto dto;
		using (var reader = doc.CreateReader())
		{
			dto = (TacoOverlayDataDto?)serializer.Deserialize(reader) ?? new TacoOverlayDataDto();
		}

		var parts = nodePath.Split('|', StringSplitOptions.RemoveEmptyEntries);
		var category = parts.Length > 0 ? parts[^1] : string.Empty;

		dto.PoisContainer ??= new TacoPoisContainerDto();

		if (string.Equals(category, PreviewCategoryNames.Trails, StringComparison.OrdinalIgnoreCase))
		{
			// Update trails
			dto.PoisContainer.Trails = rows.Select(r => new TacoTrailDto { Type = r.Name }).ToList();
		}
		else
		{
			// Update POIs for specific category or all
			var existingPois = dto.PoisContainer.Pois.ToList();

			if (!string.IsNullOrWhiteSpace(category))
				// Remove POIs of this category
				existingPois = existingPois
					.Where(p => !string.Equals(p.Type, category, StringComparison.OrdinalIgnoreCase))
					.ToList();
			else
				// Clear all if no specific category
				existingPois.Clear();

			// Add new POIs
			existingPois.AddRange(rows.Select(r => new TacoPoiDto
			{
				Type = string.IsNullOrWhiteSpace(category) ? r.Name : category,
				XPos = r.X.ToString("F2"),
				YPos = r.Y.ToString("F2"),
				ZPos = r.Z.ToString("F2")
			}));

			dto.PoisContainer.Pois = existingPois;
		}

		// Serialize back
		using var writer = new StringWriter();
		serializer.Serialize(writer, dto);
		var xmlContent = writer.ToString();

		await File.WriteAllTextAsync(filePath, xmlContent, cancellationToken);
	}

	private async Task SaveKxJsonAsync(string filePath, IReadOnlyList<GridRowDto> rows,
		CancellationToken cancellationToken)
	{
		var model = await jsonService.LoadKxJsonV1Async(filePath, cancellationToken);
		if (model is null)
			throw new InvalidOperationException("Failed to load KX JSON file.");

		var newCoordinates = rows.Select(r => new CoordinatesModel(r.Name, r.X, r.Y, r.Z)).ToArray();
		var newModel = new KxModel(model.Name, newCoordinates);

		await jsonService.SaveKxJsonV1Async(newModel, filePath, cancellationToken);
	}
}