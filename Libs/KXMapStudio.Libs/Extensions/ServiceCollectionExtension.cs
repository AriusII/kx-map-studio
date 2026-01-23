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
				.AddSingleton(typeof(IStateManagementService<>), typeof(StateManagementService<>))
				.AddSingleton<IWorkshopExplorerService, WorkshopExplorerService>()
				.AddSingleton<ISaveFileDialogService, SaveFileDialogService>()
				.AddSingleton<IGridEditorDocumentService, GridEditorDocumentService>()
				.AddSingleton<IGlobalHotkeyService, GlobalHotkeyService>();
		}

		private IServiceCollection AddViewModels()
		{
			return services
				.AddSingleton<IWorkshopExplorerViewModel, WorkshopExplorerViewModel>()
				.AddSingleton<IGridEditorViewModel, GridEditorViewModel>()
				.AddSingleton<ILeftPanelViewModel, LeftSidePanelViewModel>()
				.AddSingleton<IStatusBarViewModel, StatusBarViewModel>()
				.AddSingleton<IWorkspaceWindowViewModel, WorkspaceWindowViewModel>();
		}
	}
}