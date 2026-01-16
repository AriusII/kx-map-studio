namespace KXMapStudio.Core.Mappers;

/// <summary>
///     Maps TacO XML DTOs to domain models.
/// </summary>
public static class TacoMapper
{
	public static TacoMarkerPack MapToDomain(TacoOverlayDataDto dto)
	{
		var categories = dto.Categories.Select(MapCategory).ToList();

		var pois = dto.PoisContainer?.Pois
			.Select(MapPoi)
			.Where(p => p != null)
			.Cast<TacoPoi>()
			.ToList() ?? [];

		var trails = dto.PoisContainer?.Trails
			.Select(MapTrail)
			.Where(t => t != null)
			.Cast<TacoTrail>()
			.ToList() ?? [];

		return new TacoMarkerPack(categories, pois, trails);
	}

	private static TacoMarkerCategory MapCategory(TacoMarkerCategoryDto dto)
	{
		var subCategories = dto.SubCategories
			.Select(MapCategory)
			.ToList();

		return new TacoMarkerCategory(
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

	private static TacoPoi? MapPoi(TacoPoiDto dto)
	{
		if (string.IsNullOrEmpty(dto.Type) ||
		    !int.TryParse(dto.MapId, out var mapId) ||
		    !ParseFloat(dto.XPos, out var x) ||
		    !ParseFloat(dto.YPos, out var y) ||
		    !ParseFloat(dto.ZPos, out var z))
			return null;

		return new TacoPoi(
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

	private static TacoTrail? MapTrail(TacoTrailDto dto)
	{
		if (string.IsNullOrEmpty(dto.Type) || string.IsNullOrEmpty(dto.TrailData) ||
		    string.IsNullOrEmpty(dto.Texture)) return null;

		return new TacoTrail(
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