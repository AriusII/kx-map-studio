namespace KXMapStudio.Libs.Extensions;

public static class ServiceCollectionExtensions
{
	extension(IServiceCollection services)
	{
		public IServiceCollection AddLibsDependencies()
		{
			return services.AddServices();
		}

		private IServiceCollection AddServices()
		{
			return services;
		}
	}
}