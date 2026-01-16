namespace KXMapStudio.Core.Host;

public sealed record Initialization(ILogger<Initialization> Logger) : IHostedService
{
	public async Task StartAsync(CancellationToken cancellationToken)
	{
		var appLocation = AppDomain.CurrentDomain.BaseDirectory;
		var dataFolderPath = Path.Combine(appLocation, Constants.Settings.DataFolder);

		if (!Directory.Exists(dataFolderPath))
			try
			{
				Directory.CreateDirectory(dataFolderPath);
				Logger.LogInformation("Data folder created at: {DataFolderPath}", dataFolderPath);
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, "Failed to create Data folder at: {DataFolderPath}", dataFolderPath);
				throw;
			}
		else
			Logger.LogInformation("Data folder already exists at: {DataFolderPath}", dataFolderPath);

		await Task.CompletedTask;
	}

	public async Task StopAsync(CancellationToken cancellationToken)
	{
		await Task.CompletedTask;
	}
}