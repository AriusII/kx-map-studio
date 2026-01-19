namespace KXMapStudio.Core.Services.Serializations;

/// <summary>
///     Provides a bounded, DTO-driven XML navigation tree builder for TacO overlay documents.
/// </summary>
/// <remarks>
///     Parsing is tolerant: malformed payloads return <see langword="null" />.
/// </remarks>
public sealed class TacoXmlTreeService : ITacoXmlTreeService
{
	private static readonly XmlSerializer Serializer = new(typeof(TacoOverlayDataDto));

	/// <inheritdoc />
	public async Task<XmlTreeNodeDescriptor?> BuildOverlayTreeAsync(Stream xmlStream,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(xmlStream);

		try
		{
			// XmlSerializer is synchronous; avoid blocking the caller thread.
			var dto = await Task.Run(() => DeserializeDto(xmlStream), cancellationToken).ConfigureAwait(false);
			if (dto is null)
				return null;

			var categoriesNode = BuildCategoriesNode(dto);
			var poisNode = BuildPoisNode(dto);

			var children = new List<XmlTreeNodeDescriptor>(capacity: 2);
			if (categoriesNode is not null)
				children.Add(categoriesNode);
			if (poisNode is not null)
				children.Add(poisNode);

			return new XmlTreeNodeDescriptor("OverlayData", "OverlayData", null, children);
		}
		catch (OperationCanceledException)
		{
			throw;
		}
		catch
		{
			return null;
		}
	}

	private static TacoOverlayDataDto? DeserializeDto(Stream xmlStream)
	{
		try
		{
			if (xmlStream.CanSeek)
				xmlStream.Position = 0;

			return Serializer.Deserialize(xmlStream) as TacoOverlayDataDto;
		}
		catch
		{
			return null;
		}
	}

	private static XmlTreeNodeDescriptor? BuildCategoriesNode(TacoOverlayDataDto dto)
	{
		if (dto.Categories.Count == 0)
			return null;

		var children = dto.Categories
			.Select(c => MapCategory(c, parentKey: "MarkerCategories"))
			.Where(c => c is not null)
			.Cast<XmlTreeNodeDescriptor>()
			.ToList();

		return new XmlTreeNodeDescriptor("MarkerCategories", "MarkerCategories", children.Count, children);
	}

	private static XmlTreeNodeDescriptor? BuildPoisNode(TacoOverlayDataDto dto)
	{
		var pois = dto.PoisContainer?.Pois;
		if (pois is null || pois.Count == 0)
			return null;

		// Keep the POI list bounded: one node representing the POIs collection.
		return new XmlTreeNodeDescriptor("POIs", "POIs", pois.Count, Array.Empty<XmlTreeNodeDescriptor>());
	}

	private static XmlTreeNodeDescriptor? MapCategory(TacoMarkerCategoryDto category, string parentKey)
	{
		var key = string.IsNullOrWhiteSpace(category.Name)
			? parentKey
			: $"{parentKey}/{category.Name}";

		var displayName = string.IsNullOrWhiteSpace(category.DisplayName)
			? category.Name
			: category.DisplayName;

		if (string.IsNullOrWhiteSpace(displayName))
			displayName = "(Unnamed Category)";

		var children = category.SubCategories
			.Select(c => MapCategory(c, key))
			.Where(c => c is not null)
			.Cast<XmlTreeNodeDescriptor>()
			.ToList();

		return new XmlTreeNodeDescriptor(displayName, key, children.Count, children);
	}
}
