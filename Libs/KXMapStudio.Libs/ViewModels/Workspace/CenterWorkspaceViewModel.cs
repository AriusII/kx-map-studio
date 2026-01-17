using KXMapStudio.Core.Services;

namespace KXMapStudio.Libs.ViewModels.Workspace;

public sealed partial class CenterWorkspaceViewModel : ObservableObject, IDisposable
{
	private readonly IGridDataService _gridDataService;

	[ObservableProperty] private bool _isBusy;
	private CancellationTokenSource? _loadCts;
	[ObservableProperty] private string _status = "Aucun fichier sélectionné.";

	public CenterWorkspaceViewModel()
		: this(CreateDefaultGridDataService(), WeakReferenceMessenger.Default)
	{
	}

	private CenterWorkspaceViewModel(IGridDataService gridDataService, IMessenger messenger)
	{
		_gridDataService = gridDataService;

		messenger.Register<GridCategorySelectedMessageModel>(this,
			async void (_, message) =>
			{
				try
				{
					await LoadFromPathAsync(message.Path, message.Category);
				}
				catch (Exception e)
				{
					throw; // TODO handle exception
				}
			});
	}

	public ObservableCollection<GridRowDto> Rows { get; } = [];

	private async Task LoadFromPathAsync(string path, string category, CancellationToken cancellationToken = default)
	{
		await _loadCts?.CancelAsync()!;
		_loadCts = new CancellationTokenSource();

		try
		{
			IsBusy = true;
			Status = $"Chargement {category}...";

			var rows = await _gridDataService.LoadRowsAsync(path, category, _loadCts.Token);

			Rows.Clear();
			foreach (var row in rows) Rows.Add(row);

			Status = rows.Count == 0 ? "Aucune donnée." : $"{rows.Count} lignes chargées.";
		}
		catch (OperationCanceledException)
		{
			Status = "Chargement annulé.";
		}
		catch (Exception ex)
		{
			Status = $"Erreur: {ex.Message}";
		}
		finally
		{
			IsBusy = false;
		}
	}

	private static GridDataService CreateDefaultGridDataService()
	{
		var xmlRepo = new XmlDataRepository();
		var jsonRepo = new JsonDataRepository();
		var archiveRepo = new ArchiveDataRepository();
		var fileTypeDetector = new FileTypeDetectorService(jsonRepo, xmlRepo, archiveRepo);
		var kxService = new JsonService(jsonRepo);

		return new GridDataService(fileTypeDetector, xmlRepo, jsonRepo, archiveRepo, kxService);
	}

	public void Dispose()
	{
		_loadCts?.Dispose();
	}
}