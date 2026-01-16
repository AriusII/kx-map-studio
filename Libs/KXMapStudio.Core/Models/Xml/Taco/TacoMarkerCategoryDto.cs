namespace KXMapStudio.Core.Models.Xml.Taco;

/// <summary>
///     DTO for the MarkerCategory element.
/// </summary>
public class TacoMarkerCategoryDto
{
	[XmlAttribute("name")] public string Name { get; set; } = string.Empty;

	[XmlAttribute("DisplayName")] public string? DisplayName { get; set; }

	[XmlAttribute("IsSeparator")] public string? IsSeparator { get; set; }

	[XmlAttribute("tip-name")] public string? TipName { get; set; }

	[XmlAttribute("tip-description")] public string? TipDescription { get; set; }

	[XmlAttribute("iconFile")] public string? IconFile { get; set; }

	[XmlAttribute("iconSize")] public string? IconSize { get; set; }

	[XmlAttribute("fadeNear")] public string? FadeNear { get; set; }

	[XmlAttribute("fadeFar")] public string? FadeFar { get; set; }

	[XmlAttribute("heightOffset")] public string? HeightOffset { get; set; }

	[XmlAttribute("behavior")] public string? Behavior { get; set; }

	[XmlAttribute("minSize")] public string? MinSize { get; set; }

	[XmlAttribute("achievementId")] public string? AchievementId { get; set; }

	[XmlAttribute("achievementBit")] public string? AchievementBit { get; set; }

	[XmlAttribute("mapDisplaySize")] public string? MapDisplaySize { get; set; }

	[XmlAttribute("miniMapVisibility")] public string? MiniMapVisibility { get; set; }

	[XmlAttribute("mapVisibility")] public string? MapVisibility { get; set; }

	[XmlElement("MarkerCategory")] public List<TacoMarkerCategoryDto> SubCategories { get; set; } = new();
}