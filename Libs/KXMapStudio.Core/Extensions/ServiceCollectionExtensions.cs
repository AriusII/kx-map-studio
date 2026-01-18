namespace KXMapStudio.Core.Extensions;

public static class ServiceCollectionExtensions
{
	/// <summary>
	///     Adds all KXMapStudio.Core dependencies (hosting, HTTP, services, and repositories) to the provided service
	///     collection.
	/// </summary>
	/// <param name="services">The service collection to configure.</param>
	/// <returns>The same <see cref="IServiceCollection" /> instance so calls can be chained.</returns>
	public static IServiceCollection AddCoreDependencies(this IServiceCollection services)
	{
		ArgumentNullException.ThrowIfNull(services);

		return services
			.AddHostedService<Initialization>()
			.AddHttpClient()
			.AddCoreHttpClients()
			.AddCoreServices()
			.AddCoreRepositories();
	}

	private static IServiceCollection AddCoreHttpClients(this IServiceCollection services)
	{
		// Typed clients give better discoverability and make configuration centralized.
		services.AddHttpClient<IGuildWarsHttpClient, GuildWarsHttpClient>();
		services.AddHttpClient<IGithubHttpClient, GithubHttpClient>();
		return services;
	}

	private static IServiceCollection AddCoreServices(this IServiceCollection services)
	{
		return services
			.AddScoped<IFileReaderService, FileReaderService>()
			.AddScoped<IGw2Client, Gw2Client>()
			.AddScoped<IJsonService, JsonService>()
			.AddScoped<IXmlService, XmlService>()
			.AddScoped<IArchiveService, ArchiveService>();
	}

	private static IServiceCollection AddCoreRepositories(this IServiceCollection services)
	{
		return services
			.AddScoped<IJsonDataRepository, JsonDataRepository>()
			.AddScoped<IXmlDataRepository, XmlDataRepository>()
			.AddScoped<IArchiveDataRepository, ArchiveDataRepository>()
			.AddScoped<IFileStorageRepository, FileStorageRepository>();
	}
}