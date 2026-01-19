namespace KXMapStudio.Core.Abstractions.Services;

/// <summary>
///     Builds a bounded, DTO-driven navigation tree for TacO overlay XML files.
/// </summary>
/// <remarks>
///     This API is intended for UI exploration scenarios. It uses the known TacO DTO structure to avoid
///     unbounded / arbitrary XML traversal.
/// </remarks>
public interface ITacoXmlTreeService
{
	/// <summary>
	///     Builds a navigation tree for the provided TacO overlay XML stream.
	/// </summary>
	/// <param name="xmlStream">The XML stream (positioned at the start or any readable position).</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>
	///     A tree descriptor, or <see langword="null" /> when the XML is malformed or cannot be parsed.
	/// </returns>
	Task<XmlTreeNodeDescriptor?> BuildOverlayTreeAsync(Stream xmlStream, CancellationToken cancellationToken = default);
}
