namespace KXMapStudio.Libs.Models.Workspace;

/// <summary>
///     Message sent when a category or node is selected in the file preview.
/// </summary>
/// <param name="Path">The file path.</param>
/// <param name="NodePath">Full node path including archive paths and category (e.g., "file.taco|archive.xml|Waypoints").</param>
public sealed record GridCategorySelectedMessageModel(string Path, string NodePath);