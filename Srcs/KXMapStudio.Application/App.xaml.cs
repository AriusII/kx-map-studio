namespace KXMapStudio.Application;

public partial class App : System.Windows.Application
{
	public App()
	{
		AppHost = Host.CreateDefaultBuilder()
			.ConfigureServices(services =>
			{
				services
					.AddCoreDependencies()
					.AddLibsDependencies();
			})
			.Build();
	}

	private static IHost AppHost { get; set; } = null!;

	protected override async void OnStartup(StartupEventArgs e)
	{
		base.OnStartup(e);
	}

	protected override void OnExit(ExitEventArgs e)
	{
		AppHost.StopAsync().GetAwaiter().GetResult();
		AppHost.Dispose();

		base.OnExit(e);
	}
}