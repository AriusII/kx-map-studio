using Microsoft.Extensions.Logging;

namespace KXMapStudio.Libs.ViewModels.LeftSide;

/// <summary>
///     ViewModel for the left side panel, orchestrating file exploration and document loading into the grid editor.
/// </summary>
/// <remarks>
///     <para>
///         This ViewModel acts as an event mediator between <see cref="IWorkshopExplorerViewModel" /> (file selection source)
///         and <see cref="IGridEditorViewModel" /> (document editor target).
///     </para>
///     <para>
///         When a file is selected in the workshop explorer, the <see cref="OnFileSelected" /> handler
///         asynchronously loads the document into the grid editor, with comprehensive error logging.
///     </para>
///     <para>
///         Implements <see cref="IDisposable" /> to properly unsubscribe from events and dispose child ViewModels.
///     </para>
/// </remarks>
public sealed class LeftSidePanelViewModel : ObservableObject, ILeftPanelViewModel, IDisposable
{
	private readonly ILogger<LeftSidePanelViewModel> _logger;

	/// <summary>
	///     Initializes a new instance of the <see cref="LeftSidePanelViewModel" /> class.
	/// </summary>
	/// <param name="workshopExplorer">The workshop explorer ViewModel managing file tree navigation.</param>
	/// <param name="gridEditor">The grid editor ViewModel responsible for document editing.</param>
	/// <param name="logger">The logger for diagnostic and error tracking.</param>
	/// <exception cref="ArgumentNullException">
	///     Thrown when <paramref name="workshopExplorer" />, <paramref name="gridEditor" />,
	///     or <paramref name="logger" /> is <see langword="null" />.
	/// </exception>
	public LeftSidePanelViewModel(
		IWorkshopExplorerViewModel workshopExplorer,
		IGridEditorViewModel gridEditor,
		ILogger<LeftSidePanelViewModel> logger)
	{
		ArgumentNullException.ThrowIfNull(workshopExplorer);
		ArgumentNullException.ThrowIfNull(gridEditor);
		ArgumentNullException.ThrowIfNull(logger);

		WorkshopExplorer = workshopExplorer;
		GridEditor = gridEditor;
		_logger = logger;

		// Subscribe to file selection events if the concrete type supports it
		if (WorkshopExplorer is WorkshopExplorerViewModel explorerVm)
		{
			explorerVm.FileSelected += OnFileSelected;
			_logger.LogDebug("Subscribed to FileSelected event from WorkshopExplorerViewModel.");
		}
		else
		{
			_logger.LogWarning(
				"WorkshopExplorer does not support FileSelected event. File selection integration is unavailable.");
		}
	}

	/// <summary>
	///     Gets the workshop explorer ViewModel managing the file tree UI.
	/// </summary>
	public IWorkshopExplorerViewModel WorkshopExplorer { get; }

	/// <summary>
	///     Gets the grid editor ViewModel for document editing.
	/// </summary>
	public IGridEditorViewModel GridEditor { get; }

	/// <summary>
	///     Disposes resources, unsubscribes from events, and disposes child ViewModels.
	/// </summary>
	public void Dispose()
	{
		_logger.LogDebug("Disposing LeftSidePanelViewModel.");

		// Unsubscribe from events to prevent memory leaks
		if (WorkshopExplorer is WorkshopExplorerViewModel explorerVm)
		{
			explorerVm.FileSelected -= OnFileSelected;
			_logger.LogDebug("Unsubscribed from FileSelected event.");
		}

		// Dispose child ViewModels if they implement IDisposable
		if (WorkshopExplorer is IDisposable disposableExplorer)
		{
			disposableExplorer.Dispose();
			_logger.LogDebug("Disposed WorkshopExplorerViewModel.");
		}

		if (GridEditor is IDisposable disposableEditor)
		{
			disposableEditor.Dispose();
			_logger.LogDebug("Disposed GridEditorViewModel.");
		}

		_logger.LogInformation("LeftSidePanelViewModel disposed successfully.");
	}

	/// <summary>
	///     Handles file selection events from the workshop explorer and asynchronously loads the document into the grid editor.
	/// </summary>
	/// <param name="sender">The event source (typically <see cref="IWorkshopExplorerViewModel" />).</param>
	/// <param name="doc">The document reference to load.</param>
	/// <remarks>
	///     This is an async void event handler. Exceptions are caught and logged to prevent crashes.
	///     The grid editor handles cancellation of previous loads internally.
	/// </remarks>
	private async void OnFileSelected(object? sender, EditorDocumentReference doc)
	{
		try
		{
			_logger.LogInformation("File selected: {DisplayName} (Path: {FilePath}).", doc.DisplayName, doc.FilePath ?? doc.ArchivePath);

			await GridEditor.LoadAsync(doc);

			_logger.LogInformation("File loaded successfully: {DisplayName}.", doc.DisplayName);
		}
		catch (OperationCanceledException)
		{
			_logger.LogWarning("File load operation was canceled for document: {DisplayName}.", doc.DisplayName);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error loading document '{DisplayName}' into grid editor. Path: {FilePath}",
				doc.DisplayName, doc.FilePath ?? doc.ArchivePath);
		}
	}
}