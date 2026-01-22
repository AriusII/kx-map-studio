namespace KXMapStudio.Core.Repositories.Serializations;

/// <summary>
///     Provides XML persistence based on <see cref="XDocument" />.
/// </summary>
/// <param name="fileStorage">The underlying file storage abstraction.</param>
internal sealed class XmlRepository(IFileStorageRepository fileStorage) : IXmlRepository
{
	private readonly IFileStorageRepository _fileStorage =
		fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));

	public async Task<XDocument> LoadFromFileAsync(string filePath, CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

		if (!_fileStorage.Exists(filePath))
			return null!;

		await using var stream = await _fileStorage.LoadAsync(filePath, cancellationToken).ConfigureAwait(false);
		if (stream is null)
			return null!;

		return await LoadFromArchiveStreamAsync(stream, cancellationToken).ConfigureAwait(false);
	}

	public async Task SaveToFileAsync(XDocument document, string path, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(document);
		ArgumentException.ThrowIfNullOrWhiteSpace(path);

		await using var memory = new MemoryStream();
		await SaveToArchiveStreamAsync(document, memory, cancellationToken).ConfigureAwait(false);
		memory.Position = 0;

		await _fileStorage.SaveAsync(path, memory, cancellationToken).ConfigureAwait(false);
	}

	public async Task<XDocument> LoadFromArchiveStreamAsync(Stream stream,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(stream);

		return await XDocument.LoadAsync(stream, LoadOptions.None, cancellationToken).ConfigureAwait(false);
	}

	public async Task SaveToArchiveStreamAsync(XDocument document, Stream stream,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(document);
		ArgumentNullException.ThrowIfNull(stream);

		await using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);
		await document.SaveAsync(writer, SaveOptions.None, cancellationToken).ConfigureAwait(false);
	}
}