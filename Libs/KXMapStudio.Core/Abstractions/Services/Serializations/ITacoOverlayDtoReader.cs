namespace KXMapStudio.Core.Abstractions.Services.Serializations;

/// <summary>
/// Provides tolerant deserialization helpers for TacO <c>OverlayData</c> XML payloads.
/// </summary>
public interface ITacoOverlayDtoReader
{
	/// <summary>
	/// Deserializes a TacO <c>OverlayData</c> document from a stream.
	/// </summary>
	/// <param name="stream">The source stream positioned at the beginning of the XML payload.</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	/// <returns>The deserialized DTO, or <see langword="null" /> when the payload is missing or invalid.</returns>
	Task<TacoOverlayDataDto?> LoadAsync(Stream stream, CancellationToken cancellationToken = default);
}
