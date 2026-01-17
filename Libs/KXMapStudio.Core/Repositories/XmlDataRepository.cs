namespace KXMapStudio.Core.Repositories;

public sealed record XmlDataRepository(IFileStorageRepository FileStorage) : IXmlDataRepository
{
	public async Task<XDocument?> LoadFromFileAsync(string filePath, CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

		if (!FileStorage.Exists(filePath))
			return null;

		await using var stream = await FileStorage.LoadAsync(filePath, cancellationToken);
		if (stream is null) return null;

		return await LoadFromStreamAsync(stream, cancellationToken);
	}

	public async Task SaveToFileAsync(XDocument document, string path, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(document);
		ArgumentException.ThrowIfNullOrWhiteSpace(path);

		var directory = Path.GetDirectoryName(path);
		if (!string.IsNullOrEmpty(directory))
			Directory.CreateDirectory(directory);

		await using var memory = new MemoryStream();
		await SaveToStreamAsync(document, memory, cancellationToken);
		memory.Position = 0;

		await FileStorage.SaveAsync(path, memory, cancellationToken);
	}

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

	public async Task SaveToStreamAsync(XDocument document, Stream stream,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(document);
		ArgumentNullException.ThrowIfNull(stream);

		var settings = new XmlWriterSettings
		{
			Async = true,
			Indent = true,
			Encoding = new UTF8Encoding(false),
			CloseOutput = false
		};

		await using var writer = XmlWriter.Create(stream, settings);
		await document.SaveAsync(writer, cancellationToken);
		await writer.FlushAsync();
	}
}