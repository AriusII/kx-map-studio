namespace KXMapStudio.Libs.Extensions;

public static class ServiceCollectionExtension
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
				.AddSingleton<IWorkshopExplorerNodeService, WorkshopExplorerNodeService>()
				.AddSingleton<IFilePreviewService, FilePreviewService>()
				.AddSingleton<IGridEditorService, GridEditorService>();
		}

		private IServiceCollection AddViewModels()
		{
			return services
				.AddSingleton<IWorkshopExplorerViewModel, WorkshopExplorerViewModel>()
				.AddSingleton<IFilePreviewViewModel, FilePreviewViewModel>()
				.AddSingleton<ILeftPanelViewModel, LeftSidePanelViewModel>()
				.AddSingleton<IGridEditorViewModel, GridEditorViewModel>()
				.AddSingleton<IWorkspaceWindowViewModel, WorkspaceWindowViewModel>();
		}
	}
}