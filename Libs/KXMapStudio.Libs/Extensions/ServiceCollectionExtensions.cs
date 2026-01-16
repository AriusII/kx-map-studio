using KXMapStudio.Core.Abstractions.Services;
using KXMapStudio.Core.Services;

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
			return services
				.AddSingleton<IWorkspaceExplorerService, WorkspaceExplorerService>()
				.AddSingleton<IGridDataService, GridDataService>();
		}
	}
}