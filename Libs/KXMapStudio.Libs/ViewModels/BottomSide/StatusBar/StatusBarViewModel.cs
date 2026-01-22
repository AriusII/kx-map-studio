namespace KXMapStudio.Libs.ViewModels.BottomSide.StatusBar;

public sealed partial class StatusBarViewModel : ObservableObject, IStatusBarViewModel, IDisposable
{
	private static readonly TimeSpan ConnectionStaleTimeout = TimeSpan.FromMilliseconds(750);

	private readonly IMumbleService _mumbleService;
	private readonly CancellationTokenSource _staleCts = new();
	private readonly PeriodicTimer _staleTimer = new(TimeSpan.FromMilliseconds(250));
	private readonly SynchronizationContext? _uiContext;

	[ObservableProperty] private string _characterName = "";

	[ObservableProperty] private string _coordinatesText = "Pos: N/A";

	[ObservableProperty] private bool _isMumbleAvailable;

	/// <summary>
	///     True when MumbleLink is available AND the received data is fresh.
	/// </summary>
	[ObservableProperty] private bool _isMumbleConnected;

	private DateTimeOffset _lastEventReceivedUtc;

	[ObservableProperty] private string _mapText = "Map: N/A";

	[ObservableProperty] private string _xText = "X: -";

	[ObservableProperty] private string _yText = "Y: -";

	[ObservableProperty] private string _zText = "Z: -";

	public StatusBarViewModel(IMumbleService mumbleService)
	{
		_mumbleService = mumbleService ?? throw new ArgumentNullException(nameof(mumbleService));
		_uiContext = SynchronizationContext.Current;

		// Consider "fresh" only once we actually receive events after app start.
		_lastEventReceivedUtc = DateTimeOffset.MinValue;

		ApplySnapshot(_mumbleService.Current);
		_mumbleService.MumbleUpdated += OnMumbleUpdated;

		_ = MonitorStaleConnectionAsync(_staleCts.Token);
	}

	public void Dispose()
	{
		_mumbleService.MumbleUpdated -= OnMumbleUpdated;

		_staleCts.Cancel();
		_staleCts.Dispose();
		_staleTimer.Dispose();
	}

	private void OnMumbleUpdated(object? sender, MumbleStateModel snapshot)
	{
		// Track actual event reception time in case the snapshot timestamp isn't updated for any reason.
		_lastEventReceivedUtc = DateTimeOffset.UtcNow;

		var ctx = _uiContext;
		if (ctx != null)
		{
			ctx.Post(_ => ApplySnapshot(snapshot), null);
			return;
		}

		ApplySnapshot(snapshot);
	}

	private void ApplySnapshot(MumbleStateModel snapshot)
	{
		IsMumbleAvailable = snapshot.IsAvailable;
		UpdateConnectionState(snapshot.IsAvailable);

		if (!IsMumbleConnected)
		{
			CharacterName = IsMumbleAvailable ? "Connecting..." : "Not connected";
			MapText = "Map: N/A";
			CoordinatesText = "Pos: N/A";
			XText = "X: -";
			YText = "Y: -";
			ZText = "Z: -";
			return;
		}

		CharacterName = string.IsNullOrWhiteSpace(snapshot.CharacterName) ? "Unknown" : snapshot.CharacterName;
		MapText = $"Map: {snapshot.CurrentMapId.ToString(CultureInfo.InvariantCulture)}";

		var x = snapshot.PlayerPosition.X;
		var y = snapshot.PlayerPosition.Y;
		var z = snapshot.PlayerPosition.Z;

		XText = $"X: {Format(x)}";
		YText = $"Y: {Format(y)}";
		ZText = $"Z: {Format(z)}";

		// Keep a compact version for small layouts.
		CoordinatesText = $"Pos: {Format(x)}, {Format(y)}, {Format(z)}";
	}

	private async Task MonitorStaleConnectionAsync(CancellationToken cancellationToken)
	{
		try
		{
			while (await _staleTimer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false))
			{
				var expectedConnected = IsMumbleAvailable && IsEventFresh();
				if (IsMumbleConnected == expectedConnected)
					continue;

				var ctx = _uiContext;
				if (ctx != null)
					ctx.Post(_ => UpdateConnectionState(IsMumbleAvailable), null);
				else
					UpdateConnectionState(IsMumbleAvailable);
			}
		}
		catch (OperationCanceledException)
		{
			// Intentional no-op.
		}
	}

	private bool IsEventFresh()
	{
		if (_lastEventReceivedUtc == DateTimeOffset.MinValue)
			return false;

		return DateTimeOffset.UtcNow - _lastEventReceivedUtc <= ConnectionStaleTimeout;
	}

	private void UpdateConnectionState(bool isAvailable)
	{
		// Hard rule: we must be receiving recent updates.
		// If the GW2 client/launcher is closed, the polling loop may stop producing meaningful data.
		// The UI must never keep reporting old values as 'connected'.
		IsMumbleConnected = isAvailable && IsEventFresh();
	}

	private static string Format(double value)
	{
		return value.ToString("0.##", CultureInfo.InvariantCulture);
	}
}