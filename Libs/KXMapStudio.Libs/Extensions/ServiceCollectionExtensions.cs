namespace KXMapStudio.Libs.Extensions;

public static class ServiceCollectionExtensions
{
	extension(IServiceCollection services)
	{
		public IServiceCollection AddLibsDependencies()
		{
			return services
				.AddServices()
				.AddViewModels();
		}

		private IServiceCollection AddServices()
		{
			return services
				.AddSingleton<IWorkshopFileExplorerService, WorkshopFileExplorerService>()
				.AddSingleton<IFileExplorerNodeService, FileExplorerNodeService>();
		}

		private IServiceCollection AddViewModels()
		{
			return services
				.AddSingleton<IFileExplorerViewModel, FileExplorerViewModel>()
				.AddSingleton<ILeftPanelViewModel, LeftPanelViewModel>()
				.AddSingleton<IWorkspaceWindowViewModel, WorkspaceWindowViewModel>();
		}
	}
}