using KXMapStudio.Core.Models.IOptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KXMapStudio.Application;

public partial class App : System.Windows.Application
{
	private static IHost AppHost { get; set; } = null!;
	public App()
	{
		AppHost = Host.CreateDefaultBuilder()
			.ConfigureAppConfiguration(configuration =>
			{
				configuration
					.AddJsonFile("appsettings.json");
			})
			.ConfigureServices((contexts, services) =>
			{
				services
					.AddCoreDependencies()
					.AddLibsDependencies();
				services.Configure<SettingsOption>(
					contexts.Configuration.GetSection("Settings"));
			})
			.Build();
	}

	protected override void OnStartup(StartupEventArgs e)
	{
		base.OnStartup(e);
		AppHost.Start();
	}

	protected override void OnExit(ExitEventArgs e)
	{
		AppHost.StopAsync().GetAwaiter().GetResult();
		AppHost.Dispose();

		base.OnExit(e);
	}
}