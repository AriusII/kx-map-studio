namespace KXMapStudio.Core.Services.Serializations;

/// <summary>
///     Provides archive-centric workflows (ZIP/TACO) on top of <see cref="IArchiveDataRepository" />.
/// </summary>
/// <param name="ArchiveRepository">The underlying archive repository.</param>
/// <param name="XmlService">The XML service used to parse TACO marker packs.</param>
public sealed record ArchiveService(
	IArchiveDataRepository ArchiveRepository,
	IXmlService XmlService)
	: IArchiveService
{
	/// <summary>
	///     Enumerates all entry full names contained in the specified archive.
	/// </summary>
	/// <param name="filePath">The archive file path.</param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	/// <returns>A read-only list of entry names. Returns an empty list when the archive does not exist.</returns>
	public Task<IReadOnlyList<string>> GetEntriesAsync(string filePath,
		CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
		return ArchiveRepository.GetEntriesAsync(filePath, cancellationToken);
	}

	/// <summary>
	///     Loads and parses a TACO marker pack from a ZIP/TACO archive.
	/// </summary>
	/// <param name="filePath">The archive file path.</param>
	/// <param name="entryFullName">
	///     The full name of the entry to load. When <see langword="null" />, the first <c>.xml</c> entry is used.
	/// </param>
	/// <param name="cancellationToken">A token used to cancel the operation.</param>
	/// <returns>The parsed marker pack, or <see langword="null" /> when no suitable entry exists or parsing fails.</returns>
	public async Task<TacoMarkerPackModel?> LoadMarkerPackAsync(
		string filePath,
		string? entryFullName = null,
		CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

		try
		{
			await using var archive = await ArchiveRepository.LoadAsync(filePath, cancellationToken);

			var entry = ResolveEntry(archive, entryFullName);
			if (entry is null)
				return null;

			await using var entryStream = await entry.OpenAsync(cancellationToken);
			return await XmlService.LoadFromStreamAsync(entryStream, cancellationToken);
		}
		catch (FileNotFoundException)
		{
			return null;
		}
	}

	private static ZipArchiveEntry? ResolveEntry(ZipArchive archive, string? entryFullName)
	{
		if (!string.IsNullOrWhiteSpace(entryFullName))
			return archive.GetEntry(entryFullName);

		return archive.Entries.FirstOrDefault(e =>
			e.FullName.EndsWith(FileExtension.Xml, StringComparison.OrdinalIgnoreCase));
	}
}