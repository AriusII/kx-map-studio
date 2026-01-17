namespace KXMapStudio.Core.Extensions;

public static class ServiceCollectionExtensions
{
	extension(IServiceCollection services)
	{
		public IServiceCollection AddCoreDependencies()
		{
			return services
				.AddHostedService<Initialization>()
				.AddHttpClient()
				.AddHttpClients()
				.AddServices()
				.AddRepositories();
		}

		private IServiceCollection AddHttpClients()
		{
			return services
				.AddScoped<IGuildWarsHttpClient, GuildWarsHttpClient>()
				.AddScoped<IGithubHttpClient, GithubHttpClient>();
		}

		private IServiceCollection AddServices()
		{
			return services
				.AddScoped<IGw2Client, Gw2Client>()
				.AddScoped<IJsonService, JsonService>()
				.AddScoped<IXmlService, XmlService>()
				.AddScoped<IArchiveService, ArchiveService>();
		}

		private IServiceCollection AddRepositories()
		{
			return services
				.AddScoped<IJsonDataRepository, JsonDataRepository>()
				.AddScoped<IXmlDataRepository, XmlDataRepository>()
				.AddScoped<IArchiveDataRepository, ArchiveDataRepository>()
				.AddScoped<IFileStorageRepository, FileStorageRepository>();
		}
	}
}