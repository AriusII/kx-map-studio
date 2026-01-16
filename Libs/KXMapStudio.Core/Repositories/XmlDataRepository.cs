namespace KXMapStudio.Core.Repositories;

public sealed record XmlDataRepository : IXmlDataRepository
{
	public async Task<XDocument> LoadFromArchiveAsync(Stream stream, CancellationToken cancellationToken = default)
	{
		return await XDocument.LoadAsync(stream, LoadOptions.None, cancellationToken);
	}

	public async Task<XDocument> LoadFromFileAsync(string path, CancellationToken cancellationToken = default)
	{
		await using var stream = File.OpenRead(path);
		return await LoadFromArchiveAsync(stream, cancellationToken);
	}
}