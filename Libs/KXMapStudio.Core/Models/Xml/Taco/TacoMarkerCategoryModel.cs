namespace KXMapStudio.Core.Models.Xml.Taco;

/// <summary>
///     Represents a category of markers in a TacO marker pack.
/// </summary>
public sealed record TacoMarkerCategoryModel(
	string Name,
	string? DisplayName,
	bool IsSeparator,
	List<TacoMarkerCategoryModel> SubCategories)
{
	public string? TipName { get; init; }
	public string? TipDescription { get; init; }
	public string? IconFile { get; init; }
	public float? IconSize { get; init; }
	public float? FadeNear { get; init; }
	public float? FadeFar { get; init; }
	public float? HeightOffset { get; init; }
	public int? Behavior { get; init; }
	public float? MinSize { get; init; }
	public int? AchievementId { get; init; }
	public int? AchievementBit { get; init; }
	public int? MapDisplaySize { get; init; }
	public int? MiniMapVisibility { get; init; }
	public int? MapVisibility { get; init; }
}