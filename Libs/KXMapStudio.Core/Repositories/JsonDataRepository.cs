namespace KXMapStudio.Core.Repositories;

public sealed record JsonDataRepository(IFileStorageRepository FileStorage) : IJsonDataRepository
{
	private static readonly JsonSerializerOptions JsonOptions = new()
	{
		WriteIndented = true,
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
		Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
	};

	public async Task SaveAsync<T>(T data, string path, CancellationToken cancellationToken = default)
	{
		using var ms = new MemoryStream();
		await JsonSerializer.SerializeAsync(ms, data, JsonOptions, cancellationToken);
		ms.Position = 0;
		await FileStorage.SaveAsync(path, ms, cancellationToken);
	}

	public async Task<T?> LoadAsync<T>(string path, CancellationToken cancellationToken = default)
	{
		await using var stream = await FileStorage.LoadAsync(path, cancellationToken);
		if (stream == null)
			return default;
		return await JsonSerializer.DeserializeAsync<T>(stream, JsonOptions, cancellationToken);
	}
}