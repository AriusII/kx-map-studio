using KXMapStudio.Core.Abstractions.Repositories.Serializations;
using KXMapStudio.Core.Abstractions.Services.Serializations;

namespace KXMapStudio.Core.Services.Serializations;

public sealed record ArchiveService(
	IArchiveDataRepository ArchiveRepository,
	IXmlService XmlService)
	: IArchiveService
{
	private ZipArchive? _archive;
	private MemoryStream? _archiveMemoryStream;
	private bool _disposed;

	public bool IsArchiveLoaded => _archive is not null && !_disposed;

	public async Task LoadArchiveFromFileAsync(string filePath, CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

		var bytes = await File.ReadAllBytesAsync(filePath, cancellationToken);
		await LoadArchiveFromBytesAsync(bytes, cancellationToken);
	}

	public Task LoadArchiveFromBytesAsync(byte[] archiveData, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(archiveData);

		DisposeArchive();

		_archiveMemoryStream = new MemoryStream(archiveData);
		_archive = new ZipArchive(_archiveMemoryStream, ZipArchiveMode.Update, false);

		return Task.CompletedTask;
	}

	public IReadOnlyList<string> ListAllEntries()
	{
		ThrowIfNotLoaded();
		return _archive!.Entries.Select(e => e.FullName).ToList();
	}

	public IReadOnlyList<string> ListXmlEntries()
	{
		ThrowIfNotLoaded();
		return _archive!.Entries
			.Where(e => e.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
			.Select(e => e.FullName)
			.ToList();
	}

	public bool ContainsEntry(string entryPath)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(entryPath);
		ThrowIfNotLoaded();

		return _archive!.GetEntry(entryPath) is not null;
	}

	public async Task<Stream?> GetEntryStreamAsync(string entryPath, CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(entryPath);
		ThrowIfNotLoaded();

		var entry = _archive!.GetEntry(entryPath);
		if (entry is null) return null;

		var entryStream = entry.Open();
		var memoryStream = new MemoryStream();
		await entryStream.CopyToAsync(memoryStream, cancellationToken);
		memoryStream.Position = 0;

		await entryStream.DisposeAsync();
		return memoryStream;
	}

	public async Task<TacoMarkerPackModel?> LoadTacoMarkerPackFromEntryAsync(string xmlEntryPath,
		CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(xmlEntryPath);

		await using var stream = await GetEntryStreamAsync(xmlEntryPath, cancellationToken);
		if (stream is null) return null;

		return await XmlService.LoadFromStreamAsync(stream, cancellationToken);
	}

	public async Task<IReadOnlyList<TacoMarkerPackModel>> LoadAllTacoMarkerPacksAsync(
		CancellationToken cancellationToken = default)
	{
		ThrowIfNotLoaded();

		var result = new List<TacoMarkerPackModel>();
		var xmlEntries = ListXmlEntries();

		foreach (var xmlEntry in xmlEntries)
		{
			var pack = await LoadTacoMarkerPackFromEntryAsync(xmlEntry, cancellationToken);
			if (pack is not null)
				result.Add(pack);
		}

		return result;
	}

	public async Task AddOrUpdateXmlEntryAsync(string entryPath, TacoMarkerPackModel content,
		CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(entryPath);
		ArgumentNullException.ThrowIfNull(content);
		ThrowIfNotLoaded();

		// Supprime l'entrée existante si présente
		var existingEntry = _archive!.GetEntry(entryPath);
		existingEntry?.Delete();

		// Crée la nouvelle entrée
		var newEntry = _archive.CreateEntry(entryPath, CompressionLevel.Optimal);
		await using var entryStream = newEntry.Open();
		await XmlService.SaveToStreamAsync(content, entryStream, cancellationToken);
	}

	public async Task AddOrUpdateEntryAsync(string entryPath, byte[] content,
		CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(entryPath);
		ArgumentNullException.ThrowIfNull(content);
		ThrowIfNotLoaded();

		// Supprime l'entrée existante si présente
		var existingEntry = _archive!.GetEntry(entryPath);
		existingEntry?.Delete();

		// Crée la nouvelle entrée
		var newEntry = _archive.CreateEntry(entryPath, CompressionLevel.Optimal);
		await using var entryStream = newEntry.Open();
		await entryStream.WriteAsync(content, cancellationToken);
	}

	public Task RemoveEntryAsync(string entryPath, CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(entryPath);
		ThrowIfNotLoaded();

		var entry = _archive!.GetEntry(entryPath);
		entry?.Delete();

		return Task.CompletedTask;
	}

	public async Task SaveArchiveToFileAsync(string filePath, CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
		ThrowIfNotLoaded();

		// Flush les modifications dans le MemoryStream
		_archive!.Dispose();
		_archive = null;

		// Sauvegarde sur disque
		var directory = Path.GetDirectoryName(filePath);
		if (!string.IsNullOrEmpty(directory))
			Directory.CreateDirectory(directory);

		var bytes = _archiveMemoryStream!.ToArray();
		await File.WriteAllBytesAsync(filePath, bytes, cancellationToken);

		// Recharge l'archive pour continuer à travailler dessus
		await LoadArchiveFromBytesAsync(bytes, cancellationToken);
	}

	public Task<byte[]> GetArchiveBytesAsync(CancellationToken cancellationToken = default)
	{
		ThrowIfNotLoaded();

		// Flush les modifications dans le MemoryStream
		if (_archive is not null)
		{
			_archive.Dispose();
			_archive = null;
		}

		var bytes = _archiveMemoryStream!.ToArray();

		// Recharge l'archive pour continuer à travailler dessus
		_archiveMemoryStream = new MemoryStream(bytes);
		_archive = new ZipArchive(_archiveMemoryStream, ZipArchiveMode.Update, false);

		return Task.FromResult(bytes);
	}

	public void Dispose()
	{
		if (_disposed) return;
		DisposeArchive();
		_disposed = true;
	}

	private void DisposeArchive()
	{
		_archive?.Dispose();
		_archive = null;
		_archiveMemoryStream?.Dispose();
		_archiveMemoryStream = null;
	}

	private void ThrowIfNotLoaded()
	{
		if (!IsArchiveLoaded)
			throw new InvalidOperationException("Aucune archive n'est actuellement chargée.");
	}
}