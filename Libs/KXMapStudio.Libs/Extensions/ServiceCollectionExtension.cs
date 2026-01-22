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
				.AddSingleton<ISnackbarMessageQueue, SnackbarMessageQueue>()
				.AddSingleton(typeof(IStateManagementService<>), typeof(StateManagementService<>))
				.AddSingleton<IWorkshopExplorerScanner, WorkshopExplorerScanner>()
				.AddSingleton<IWorkshopExplorerService, WorkshopExplorerService>()
				.AddSingleton<IWorkshopExplorerNodeService, WorkshopExplorerNodeService>()
				.AddSingleton<IFilePreviewService, FilePreviewService>()
				.AddSingleton<ISaveFileDialogService, SaveFileDialogService>()
				.AddSingleton<IGridEditorDocumentService, GridEditorDocumentService>();
		}

		private IServiceCollection AddViewModels()
		{
			return services
				.AddSingleton<IWorkshopExplorerViewModel, WorkshopExplorerViewModel>()
				.AddSingleton<IFilePreviewViewModel, FilePreviewViewModel>()
				.AddSingleton<IGridEditorViewModel, GridEditorViewModel>()
				.AddSingleton<ILeftPanelViewModel, LeftSidePanelViewModel>()
				.AddSingleton<IMainMenuViewModel, MainMenuViewModel>()
				.AddSingleton<IStatusBarViewModel, StatusBarViewModel>()
				.AddSingleton<IWorkspaceWindowViewModel, WorkspaceWindowViewModel>();
		}
	}
}