namespace KXMapStudio.Core.Repositories;

/// <summary>
///     Provides XML persistence based on <see cref="XDocument" />.
/// </summary>
/// <param name="FileStorage">The underlying file storage abstraction.</param>
public sealed record XmlDataRepository(IFileStorageRepository FileStorage) : IXmlDataRepository
{
	private static readonly XmlWriterSettings WriterSettings = new()
	{
		Async = true,
		Indent = true,
		Encoding = new UTF8Encoding(false),
		CloseOutput = false
	};

	/// <inheritdoc />
	public async Task<XDocument?> LoadFromFileAsync(string filePath, CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

		if (!FileStorage.Exists(filePath))
			return null;

		await using var stream = await FileStorage.LoadAsync(filePath, cancellationToken);
		if (stream is null)
			return null;

		return await LoadFromStreamAsync(stream, cancellationToken);
	}

	/// <inheritdoc />
	public async Task SaveToFileAsync(XDocument document, string path, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(document);
		ArgumentException.ThrowIfNullOrWhiteSpace(path);

		await using var memory = new MemoryStream();
		await SaveToStreamAsync(document, memory, cancellationToken);
		memory.Position = 0;

		await FileStorage.SaveAsync(path, memory, cancellationToken);
	}

	/// <inheritdoc />
	public async Task<XDocument?> LoadFromStreamAsync(Stream stream, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(stream);

		try
		{
			return await XDocument.LoadAsync(stream, LoadOptions.None, cancellationToken);
		}
		catch
		{
			return null;
		}
	}

	/// <inheritdoc />
	public async Task SaveToStreamAsync(XDocument document, Stream stream,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(document);
		ArgumentNullException.ThrowIfNull(stream);

		await using var writer = XmlWriter.Create(stream, WriterSettings);
		await document.SaveAsync(writer, cancellationToken);
		await writer.FlushAsync();
	}
}