namespace KXMapStudio.Core.Models.Xml.Taco.Dtos;

/// <summary>
///     Represents the <c>MarkerCategory</c> XML element used by TacO marker packs.
/// </summary>
public sealed class TacoMarkerCategoryDto
{
	/// <summary>
	///     Gets or sets the category identifier.
	/// </summary>
	[XmlAttribute("name")]
	public string Name { get; set; } = string.Empty;

	/// <summary>
	///     Gets or sets the display name.
	/// </summary>
	[XmlAttribute("displayName")]
	public string? DisplayName { get; set; }

	/// <summary>
	///     Gets or sets the separator flag (<c>"1"</c> or <c>"0"</c>).
	/// </summary>
	[XmlAttribute("IsSeparator")]
	public string? IsSeparator { get; set; }

	/// <summary>
	///     Gets or sets the tooltip title.
	/// </summary>
	[XmlAttribute("tip-name")]
	public string? TipName { get; set; }

	/// <summary>
	///     Gets or sets the tooltip description.
	/// </summary>
	[XmlAttribute("tip-description")]
	public string? TipDescription { get; set; }

	/// <summary>
	///     Gets or sets the icon file path.
	/// </summary>
	[XmlAttribute("iconFile")]
	public string? IconFile { get; set; }

	/// <summary>
	///     Gets or sets the icon size (culture-invariant string).
	/// </summary>
	[XmlAttribute("iconSize")]
	public string? IconSize { get; set; }

	/// <summary>
	///     Gets or sets the near fade distance.
	/// </summary>
	[XmlAttribute("fadeNear")]
	public string? FadeNear { get; set; }

	/// <summary>
	///     Gets or sets the far fade distance.
	/// </summary>
	[XmlAttribute("fadeFar")]
	public string? FadeFar { get; set; }

	/// <summary>
	///     Gets or sets the height offset.
	/// </summary>
	[XmlAttribute("heightOffset")]
	public string? HeightOffset { get; set; }

	/// <summary>
	///     Gets or sets the behavior flags.
	/// </summary>
	[XmlAttribute("behavior")]
	public string? Behavior { get; set; }

	/// <summary>
	///     Gets or sets the minimum size.
	/// </summary>
	[XmlAttribute("minSize")]
	public string? MinSize { get; set; }

	/// <summary>
	///     Gets or sets the achievement identifier.
	/// </summary>
	[XmlAttribute("achievementId")]
	public string? AchievementId { get; set; }

	/// <summary>
	///     Gets or sets the achievement bit index.
	/// </summary>
	[XmlAttribute("achievementBit")]
	public string? AchievementBit { get; set; }

	/// <summary>
	///     Gets or sets the map display size.
	/// </summary>
	[XmlAttribute("mapDisplaySize")]
	public string? MapDisplaySize { get; set; }

	/// <summary>
	///     Gets or sets the minimap visibility value.
	/// </summary>
	[XmlAttribute("miniMapVisibility")]
	public string? MiniMapVisibility { get; set; }

	/// <summary>
	///     Gets or sets the map visibility value.
	/// </summary>
	[XmlAttribute("mapVisibility")]
	public string? MapVisibility { get; set; }

	/// <summary>
	///     Gets or sets the nested marker categories.
	/// </summary>
	[XmlElement("MarkerCategory")]
	public List<TacoMarkerCategoryDto> SubCategories { get; set; } = [];
}