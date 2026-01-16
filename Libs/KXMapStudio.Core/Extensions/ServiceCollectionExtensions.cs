using KXMapStudio.Core.Services.Serializations;

namespace KXMapStudio.Core.Extensions;

public static class ServiceCollectionExtensions
{
	extension(IServiceCollection services)
	{
		public IServiceCollection AddCoreDependencies()
		{
			return services
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
				.AddScoped<IJsonService, JsonService>()
				.AddScoped<ITacoService, TacoService>();
		}

		private IServiceCollection AddRepositories()
		{
			return services
				.AddScoped<IJsonDataRepository, JsonDataRepository>()
				.AddScoped<IXmlDataRepository, XmlDataRepository>()
				.AddScoped<IArchiveRepository, ArchiveRepository>();
		}
	}
}