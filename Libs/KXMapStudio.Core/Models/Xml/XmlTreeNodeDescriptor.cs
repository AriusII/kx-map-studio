namespace KXMapStudio.Core.Models.Xml;

/// <summary>
///     Describes a navigable XML node for UI tree exploration.
/// </summary>
/// <param name="DisplayName">The user-facing label for the node.</param>
/// <param name="Key">
///     A stable key that identifies the node within a document (for example a category name or an XPath-like
///     segment string). This is intentionally UI-agnostic.
/// </param>
/// <param name="Count">
///     Optional amount of items represented by this node (for example the number of POIs). When <see langword="null" />
///     the information is unknown.
/// </param>
/// <param name="Children">Child nodes (already materialized by the provider).</param>
public sealed record XmlTreeNodeDescriptor(
	string DisplayName,
	string Key,
	int? Count,
	IReadOnlyList<XmlTreeNodeDescriptor> Children);
