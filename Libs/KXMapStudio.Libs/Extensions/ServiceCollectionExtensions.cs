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
				.AddSingleton<IFileSystemService>(sp =>
				{
					var workspaceService = sp.GetRequiredService<IWorkspaceExplorerService>();
					return new FileSystemService(workspaceService.DataFolder);
				})
				.AddSingleton<IGridDataService, GridDataService>()
				.AddSingleton<IGridEditorService, GridEditorService>()
				.AddSingleton<IFilePreviewService, FilePreviewService>();
		}
	}
}