namespace KXMapStudio.Application;

public sealed partial class App
{
	public App()
	{
		AppHost = Host.CreateDefaultBuilder()
			.ConfigureAppConfiguration(configuration => { configuration.AddJsonFile("appsettings.json"); })
			.ConfigureServices((contexts, services) =>
			{
				services
					.AddCoreDependencies()
					.AddLibsDependencies();
				services.AddTransient<WorkspaceWindowViewModel>();
				services.AddTransient<WorkspaceWindow>();

				services.Configure<SettingsOption>(contexts.Configuration.GetSection("Settings"));
			})
			.Build();
	}

	private static IHost AppHost { get; set; } = null!;

	protected override void OnStartup(StartupEventArgs e)
	{
		ForceCulture("en-US");
		base.OnStartup(e);
		AppHost.Start();

		var mainWindow = AppHost.Services.GetRequiredService<WorkspaceWindow>();
		MainWindow = mainWindow;
		mainWindow.Show();
	}

	private static void ForceCulture(string cultureName)
	{
		var culture = CultureInfo.GetCultureInfo(cultureName);

		CultureInfo.DefaultThreadCurrentCulture = culture;
		CultureInfo.DefaultThreadCurrentUICulture = culture;
		Thread.CurrentThread.CurrentCulture = culture;
		Thread.CurrentThread.CurrentUICulture = culture;
	}

	protected override void OnExit(ExitEventArgs e)
	{
		AppHost.StopAsync().GetAwaiter().GetResult();
		AppHost.Dispose();

		base.OnExit(e);
	}
}