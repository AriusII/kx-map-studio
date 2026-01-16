namespace KXMapStudio.Core.Repositories;

public sealed record JsonDataRepository : IJsonDataRepository
{
	private static readonly JsonSerializerOptions JsonOptions = new()
	{
		WriteIndented = true,
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
		Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
	};

	public async Task SaveDataAsync<T>(T data, string outputFile, CancellationToken cancellationToken = default)
	{
		var directory = Path.GetDirectoryName(outputFile);
		if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);

		await using var createStream = File.Create(outputFile);
		await JsonSerializer.SerializeAsync(createStream, data, JsonOptions, cancellationToken);
	}

	public async Task<T?> LoadDataAsync<T>(string path, CancellationToken cancellationToken = default)
	{
		if (!File.Exists(path)) return default;

		await using var openStream = File.OpenRead(path);
		return await JsonSerializer.DeserializeAsync<T>(openStream, JsonOptions, cancellationToken);
	}
}