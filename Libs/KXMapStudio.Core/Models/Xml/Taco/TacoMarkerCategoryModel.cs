namespace KXMapStudio.Core.Models.Xml.Taco;

/// <summary>
///     Represents a marker category in a TacO marker pack.
/// </summary>
/// <param name="Name">The category identifier.</param>
/// <param name="DisplayName">The optional display name.</param>
/// <param name="IsSeparator">Indicates whether this category is a separator.</param>
/// <param name="SubCategories">The nested subcategories.</param>
public sealed record TacoMarkerCategoryModel(
	string Name,
	string? DisplayName,
	bool IsSeparator,
	IReadOnlyList<TacoMarkerCategoryModel> SubCategories)
{
	/// <summary>
	///     Gets the tooltip title.
	/// </summary>
	public string? TipName { get; init; }

	/// <summary>
	///     Gets the tooltip description.
	/// </summary>
	public string? TipDescription { get; init; }

	/// <summary>
	///     Gets the icon file path.
	/// </summary>
	public string? IconFile { get; init; }

	/// <summary>
	///     Gets the icon size.
	/// </summary>
	public float? IconSize { get; init; }

	/// <summary>
	///     Gets the near fade distance.
	/// </summary>
	public float? FadeNear { get; init; }

	/// <summary>
	///     Gets the far fade distance.
	/// </summary>
	public float? FadeFar { get; init; }

	/// <summary>
	///     Gets the marker height offset.
	/// </summary>
	public float? HeightOffset { get; init; }

	/// <summary>
	///     Gets the behavior flags.
	/// </summary>
	public int? Behavior { get; init; }

	/// <summary>
	///     Gets the minimum size.
	/// </summary>
	public float? MinSize { get; init; }

	/// <summary>
	///     Gets the related achievement identifier.
	/// </summary>
	public int? AchievementId { get; init; }

	/// <summary>
	///     Gets the achievement bit index.
	/// </summary>
	public int? AchievementBit { get; init; }

	/// <summary>
	///     Gets the map display size.
	/// </summary>
	public int? MapDisplaySize { get; init; }

	/// <summary>
	///     Gets the minimap visibility value.
	/// </summary>
	public int? MiniMapVisibility { get; init; }

	/// <summary>
	///     Gets the map visibility value.
	/// </summary>
	public int? MapVisibility { get; init; }
}