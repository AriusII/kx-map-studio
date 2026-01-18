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
				.AddSingleton<IWorkshopExplorerService, WorkshopExplorerService>()
				.AddSingleton<IFileExplorerNodeService, FileExplorerNodeService>()
				.AddSingleton<IFilePreviewService, FilePreviewService>();
		}

		private IServiceCollection AddViewModels()
		{
			return services
				.AddSingleton<IFileExplorerViewModel, FileExplorerViewModel>()
				.AddSingleton<IFilePreviewViewModel, FilePreviewViewModel>()
				.AddSingleton<ILeftPanelViewModel, LeftPanelViewModel>()
				.AddSingleton<IWorkspaceWindowViewModel, WorkspaceWindowViewModel>();
		}
	}
}