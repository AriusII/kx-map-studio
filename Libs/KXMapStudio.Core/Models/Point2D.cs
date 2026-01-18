namespace KXMapStudio.Core.Models;

/// <summary>
///     Represents an immutable 2D point.
/// </summary>
/// <param name="X">The X coordinate.</param>
/// <param name="Y">The Y coordinate.</param>
public sealed record Point2D(double X, double Y);