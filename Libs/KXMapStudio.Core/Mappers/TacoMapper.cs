namespace KXMapStudio.Core.Mappers;

/// <summary>
///     Provides mapping helpers between TacO XML DTOs and Core domain models.
/// </summary>
/// <remarks>
///     This mapper is intentionally tolerant: invalid POIs/trails are discarded during conversion.
/// </remarks>
public static class TacoMapper
{
	/// <summary>
	///     Converts a TacO overlay DTO into a domain marker pack.
	/// </summary>
	/// <param name="dto">The source DTO.</param>
	/// <returns>A domain marker pack.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="dto" /> is <see langword="null" />.</exception>
	public static TacoMarkerPackModel MapToDomain(TacoOverlayDataDto dto)
	{
		ArgumentNullException.ThrowIfNull(dto);

		var categories = dto.Categories.Select(MapCategory).ToList();

		var pois = new List<TacoPoiModel>();
		var trails = new List<TacoTrailModel>();

		if (dto.PoisContainer is not null)
		{
			if (dto.PoisContainer.Pois is { Count: > 0 })
			{
				pois = new List<TacoPoiModel>(dto.PoisContainer.Pois.Count);
				foreach (var poiDto in dto.PoisContainer.Pois)
				{
					var poi = MapPoi(poiDto);
					if (poi is not null)
						pois.Add(poi);
				}
			}

			if (dto.PoisContainer.Trails is { Count: > 0 })
			{
				trails = new List<TacoTrailModel>(dto.PoisContainer.Trails.Count);
				foreach (var trailDto in dto.PoisContainer.Trails)
				{
					var trail = MapTrail(trailDto);
					if (trail is not null)
						trails.Add(trail);
				}
			}
		}

		return new TacoMarkerPackModel(categories, pois, trails);
	}

	/// <summary>
	///     Converts a domain marker pack into a TacO overlay DTO.
	/// </summary>
	/// <param name="model">The domain marker pack.</param>
	/// <returns>A DTO suitable for XML serialization.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="model" /> is <see langword="null" />.</exception>
	public static TacoOverlayDataDto MapToDto(TacoMarkerPackModel model)
	{
		ArgumentNullException.ThrowIfNull(model);

		return new TacoOverlayDataDto
		{
			Categories = model.Categories.Select(MapCategoryDto).ToList(),
			PoisContainer = new TacoPoisContainerDto
			{
				Pois = model.Pois.Select(MapPoiDto).ToList(),
				Trails = model.Trails.Select(MapTrailDto).ToList()
			}
		};
	}

	private static TacoMarkerCategoryDto MapCategoryDto(TacoMarkerCategoryModel model)
	{
		return new TacoMarkerCategoryDto
		{
			Name = model.Name,
			DisplayName = model.DisplayName,
			IsSeparator = model.IsSeparator ? "1" : "0",
			TipName = model.TipName,
			TipDescription = model.TipDescription,
			IconFile = model.IconFile,
			IconSize = FormatFloat(model.IconSize),
			FadeNear = FormatFloat(model.FadeNear),
			FadeFar = FormatFloat(model.FadeFar),
			HeightOffset = FormatFloat(model.HeightOffset),
			Behavior = FormatInt(model.Behavior),
			MinSize = FormatFloat(model.MinSize),
			AchievementId = FormatInt(model.AchievementId),
			AchievementBit = FormatInt(model.AchievementBit),
			MapDisplaySize = FormatInt(model.MapDisplaySize),
			MiniMapVisibility = FormatInt(model.MiniMapVisibility),
			MapVisibility = FormatInt(model.MapVisibility),
			SubCategories = model.SubCategories.Select(MapCategoryDto).ToList()
		};
	}

	private static TacoPoiDto MapPoiDto(TacoPoiModel model)
	{
		return new TacoPoiDto
		{
			Guid = model.Guid,
			MapId = model.MapId.ToString(CultureInfo.InvariantCulture),
			XPos = model.X.ToString(CultureInfo.InvariantCulture),
			YPos = model.Y.ToString(CultureInfo.InvariantCulture),
			ZPos = model.Z.ToString(CultureInfo.InvariantCulture),
			Type = model.Type,
			IconFile = model.IconFile,
			IconSize = FormatFloat(model.IconSize)
		};
	}

	private static TacoTrailDto MapTrailDto(TacoTrailModel model)
	{
		return new TacoTrailDto
		{
			Guid = model.Guid,
			Type = model.Type,
			TrailData = model.TrailData,
			Texture = model.Texture,
			AnimSpeed = FormatFloat(model.AnimSpeed),
			FadeNear = FormatFloat(model.FadeNear),
			FadeFar = FormatFloat(model.FadeFar)
		};
	}

	private static string? FormatFloat(float? value)
	{
		return value?.ToString(CultureInfo.InvariantCulture);
	}

	private static string? FormatInt(int? value)
	{
		return value?.ToString(CultureInfo.InvariantCulture);
	}

	private static TacoMarkerCategoryModel MapCategory(TacoMarkerCategoryDto dto)
	{
		var subCategories = dto.SubCategories
			.Select(MapCategory)
			.ToList();

		return new TacoMarkerCategoryModel(
			dto.Name,
			dto.DisplayName,
			dto.IsSeparator == "1",
			subCategories)
		{
			TipName = dto.TipName,
			TipDescription = dto.TipDescription,
			IconFile = dto.IconFile,
			IconSize = ParseFloat(dto.IconSize),
			FadeNear = ParseFloat(dto.FadeNear),
			FadeFar = ParseFloat(dto.FadeFar),
			HeightOffset = ParseFloat(dto.HeightOffset),
			Behavior = ParseInt(dto.Behavior),
			MinSize = ParseFloat(dto.MinSize),
			AchievementId = ParseInt(dto.AchievementId),
			AchievementBit = ParseInt(dto.AchievementBit),
			MapDisplaySize = ParseInt(dto.MapDisplaySize),
			MiniMapVisibility = ParseInt(dto.MiniMapVisibility),
			MapVisibility = ParseInt(dto.MapVisibility)
		};
	}

	private static TacoPoiModel? MapPoi(TacoPoiDto dto)
	{
		if (string.IsNullOrEmpty(dto.Type) ||
		    !int.TryParse(dto.MapId, out var mapId) ||
		    !ParseFloat(dto.XPos, out var x) ||
		    !ParseFloat(dto.YPos, out var y) ||
		    !ParseFloat(dto.ZPos, out var z))
			return null;

		return new TacoPoiModel(
			dto.Guid ?? string.Empty,
			mapId,
			x,
			y,
			z,
			dto.Type)
		{
			IconFile = dto.IconFile,
			IconSize = ParseFloat(dto.IconSize)
		};
	}

	private static TacoTrailModel? MapTrail(TacoTrailDto dto)
	{
		if (string.IsNullOrEmpty(dto.Type) || string.IsNullOrEmpty(dto.TrailData) ||
		    string.IsNullOrEmpty(dto.Texture)) return null;

		return new TacoTrailModel(
			dto.Guid ?? string.Empty,
			dto.Type,
			dto.TrailData,
			dto.Texture)
		{
			AnimSpeed = ParseFloat(dto.AnimSpeed),
			FadeNear = ParseFloat(dto.FadeNear),
			FadeFar = ParseFloat(dto.FadeFar)
		};
	}

	private static float? ParseFloat(string? value)
	{
		if (float.TryParse(value, CultureInfo.InvariantCulture, out var result))
			return result;

		return null;
	}

	private static bool ParseFloat(string? value, out float result)
	{
		return float.TryParse(value, CultureInfo.InvariantCulture, out result);
	}

	private static int? ParseInt(string? value)
	{
		if (int.TryParse(value, out var result))
			return result;

		return null;
	}
}