using System.Globalization;

namespace KXMapStudio.Core.Mappers;

/// <summary>
///     Provides TacO overlay XML mapping between <see cref="XDocument" /> and <see cref="MarkerModel" />.
/// </summary>
internal static class XmlMapper
{
	/// <summary>
	///     Deserializes a TacO overlay XML document into a marker pack model.
	/// </summary>
	/// <param name="document">The source XML document.</param>
	/// <returns>
	///     A non-null <see cref="MarkerModel" /> instance.
	///     Returns an empty marker pack when the XML does not match the expected TacO shape.
	/// </returns>
	internal static MarkerModel Deserialize(XDocument document)
	{
		ArgumentNullException.ThrowIfNull(document);

		try
		{
			var root = document.Root;
			if (root is null || !root.Name.LocalName.Equals("OverlayData", StringComparison.Ordinal))
				return Empty();

			var categories = ParseCategories(root);
			var pois = ParsePois(root);
			var trails = ParseTrails(root);

			return new MarkerModel(categories, pois, trails);
		}
		catch
		{
			return Empty();
		}
	}

	/// <summary>
	///     Serializes a marker pack model into a TacO overlay XML document.
	/// </summary>
	/// <param name="model">The marker pack model.</param>
	/// <returns>A non-null <see cref="XDocument" /> representing the TacO overlay.</returns>
	internal static XDocument Serialize(MarkerModel model)
	{
		ArgumentNullException.ThrowIfNull(model);

		var root = new XElement("OverlayData");

		foreach (var category in model.Categories)
			root.Add(SerializeCategory(category));

		foreach (var poi in model.Pois)
			root.Add(SerializePoi(poi));

		foreach (var trail in model.Trails)
			root.Add(SerializeTrail(trail));

		return new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root);
	}

	private static MarkerModel Empty()
		=> new([], [], []);

	private static IReadOnlyList<MarkerCategoryModel> ParseCategories(XElement overlayData)
	{
		var list = new List<MarkerCategoryModel>();

		foreach (var element in overlayData.Elements().Where(e => e.Name.LocalName.Equals("MarkerCategory", StringComparison.Ordinal)))
			list.Add(ParseCategory(element));

		return list.Count == 0 ? Array.Empty<MarkerCategoryModel>() : list;
	}

	private static MarkerCategoryModel ParseCategory(XElement element)
	{
		var model = new MarkerCategoryModel(
			(string?)element.Attribute("name") ?? string.Empty,
			(string?)element.Attribute("displayName"),
			ParseBool((string?)element.Attribute("isSeparator")),
			Array.Empty<MarkerCategoryModel>());

		var subs = element.Elements()
			.Where(e => e.Name.LocalName.Equals("MarkerCategory", StringComparison.Ordinal))
			.Select(ParseCategory)
			.ToList();

		return model with
		{
			SubCategories = subs.Count == 0 ? Array.Empty<MarkerCategoryModel>() : subs,
			TipName = (string?)element.Attribute("tipName"),
			TipDescription = (string?)element.Attribute("tipDescription"),
			IconFile = (string?)element.Attribute("iconFile"),
			IconSize = ParseFloat((string?)element.Attribute("iconSize")),
			FadeNear = ParseFloat((string?)element.Attribute("fadeNear")),
			FadeFar = ParseFloat((string?)element.Attribute("fadeFar")),
			HeightOffset = ParseFloat((string?)element.Attribute("heightOffset")),
			Behavior = ParseInt((string?)element.Attribute("behavior")),
			MinSize = ParseFloat((string?)element.Attribute("minSize")),
			AchievementId = ParseInt((string?)element.Attribute("achievementId")),
			AchievementBit = ParseInt((string?)element.Attribute("achievementBit")),
			MapDisplaySize = ParseInt((string?)element.Attribute("mapDisplaySize")),
			MiniMapVisibility = ParseInt((string?)element.Attribute("miniMapVisibility")),
			MapVisibility = ParseInt((string?)element.Attribute("mapVisibility"))
		};
	}

	private static IReadOnlyList<PoiModel> ParsePois(XElement overlayData)
	{
		var list = new List<PoiModel>();

		foreach (var e in overlayData.Descendants().Where(d => d.Name.LocalName.Equals("POI", StringComparison.Ordinal)))
		{
			var poi = new PoiModel(
				(string?)e.Attribute("guid") ?? string.Empty,
				ParseIntStrict((string?)e.Attribute("mapid")),
				ParseFloatStrict((string?)e.Attribute("xpos")),
				ParseFloatStrict((string?)e.Attribute("ypos")),
				ParseFloatStrict((string?)e.Attribute("zpos")),
				(string?)e.Attribute("type") ?? string.Empty)
			{
				IconFile = (string?)e.Attribute("iconFile"),
				IconSize = ParseFloat((string?)e.Attribute("iconSize"))
			};

			list.Add(poi);
		}

		return list.Count == 0 ? Array.Empty<PoiModel>() : list;
	}

	private static IReadOnlyList<TrailModel> ParseTrails(XElement overlayData)
	{
		var list = new List<TrailModel>();

		foreach (var e in overlayData.Descendants().Where(d => d.Name.LocalName.Equals("Trail", StringComparison.Ordinal)))
		{
			var trail = new TrailModel(
				(string?)e.Attribute("guid") ?? string.Empty,
				(string?)e.Attribute("type") ?? string.Empty,
				(string?)e.Attribute("trailData") ?? string.Empty,
				(string?)e.Attribute("texture") ?? string.Empty)
			{
				AnimSpeed = ParseFloat((string?)e.Attribute("animSpeed")),
				FadeNear = ParseFloat((string?)e.Attribute("fadeNear")),
				FadeFar = ParseFloat((string?)e.Attribute("fadeFar"))
			};

			list.Add(trail);
		}

		return list.Count == 0 ? Array.Empty<TrailModel>() : list;
	}

	private static XElement SerializeCategory(MarkerCategoryModel model)
	{
		var e = new XElement("MarkerCategory");
		e.SetAttributeValue("name", model.Name);

		if (!string.IsNullOrWhiteSpace(model.DisplayName))
			e.SetAttributeValue("displayName", model.DisplayName);

		if (model.IsSeparator)
			e.SetAttributeValue("isSeparator", "1");

		SetOptional(e, "tipName", model.TipName);
		SetOptional(e, "tipDescription", model.TipDescription);
		SetOptional(e, "iconFile", model.IconFile);
		SetOptional(e, "iconSize", model.IconSize);
		SetOptional(e, "fadeNear", model.FadeNear);
		SetOptional(e, "fadeFar", model.FadeFar);
		SetOptional(e, "heightOffset", model.HeightOffset);
		SetOptional(e, "behavior", model.Behavior);
		SetOptional(e, "minSize", model.MinSize);
		SetOptional(e, "achievementId", model.AchievementId);
		SetOptional(e, "achievementBit", model.AchievementBit);
		SetOptional(e, "mapDisplaySize", model.MapDisplaySize);
		SetOptional(e, "miniMapVisibility", model.MiniMapVisibility);
		SetOptional(e, "mapVisibility", model.MapVisibility);

		foreach (var sub in model.SubCategories)
			e.Add(SerializeCategory(sub));

		return e;
	}

	private static XElement SerializePoi(PoiModel model)
	{
		var e = new XElement("POI");
		e.SetAttributeValue("guid", model.Guid);
		e.SetAttributeValue("mapid", model.MapId.ToString(CultureInfo.InvariantCulture));
		e.SetAttributeValue("xpos", model.X.ToString(CultureInfo.InvariantCulture));
		e.SetAttributeValue("ypos", model.Y.ToString(CultureInfo.InvariantCulture));
		e.SetAttributeValue("zpos", model.Z.ToString(CultureInfo.InvariantCulture));
		e.SetAttributeValue("type", model.Type);

		SetOptional(e, "iconFile", model.IconFile);
		SetOptional(e, "iconSize", model.IconSize);

		return e;
	}

	private static XElement SerializeTrail(TrailModel model)
	{
		var e = new XElement("Trail");
		e.SetAttributeValue("guid", model.Guid);
		e.SetAttributeValue("type", model.Type);
		e.SetAttributeValue("trailData", model.TrailData);
		e.SetAttributeValue("texture", model.Texture);

		SetOptional(e, "animSpeed", model.AnimSpeed);
		SetOptional(e, "fadeNear", model.FadeNear);
		SetOptional(e, "fadeFar", model.FadeFar);

		return e;
	}

	private static void SetOptional(XElement element, string attributeName, string? value)
	{
		if (!string.IsNullOrWhiteSpace(value))
			element.SetAttributeValue(attributeName, value);
	}

	private static void SetOptional(XElement element, string attributeName, int? value)
	{
		if (value.HasValue)
			element.SetAttributeValue(attributeName, value.Value.ToString(CultureInfo.InvariantCulture));
	}

	private static void SetOptional(XElement element, string attributeName, float? value)
	{
		if (value.HasValue)
			element.SetAttributeValue(attributeName, value.Value.ToString(CultureInfo.InvariantCulture));
	}

	private static bool ParseBool(string? value)
		=> !string.IsNullOrWhiteSpace(value) &&
		   (value.Equals("1", StringComparison.Ordinal) || value.Equals("true", StringComparison.OrdinalIgnoreCase));

	private static int? ParseInt(string? value)
		=> int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i) ? i : null;

	private static float? ParseFloat(string? value)
		=> float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var f) ? f : null;

	private static int ParseIntStrict(string? value)
		=> int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i) ? i : 0;

	private static float ParseFloatStrict(string? value)
		=> float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var f) ? f : 0f;
}