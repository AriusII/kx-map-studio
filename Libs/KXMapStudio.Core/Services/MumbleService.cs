namespace KXMapStudio.Core.Services;

/// <summary>
///     Provides polling-based access to Guild Wars 2 MumbleLink state via <see cref="IGw2Client" />.
/// </summary>
/// <remarks>
///     This service is intentionally defensive:
///     <list type="bullet">
///         <item>
///             <description>Transient failures are swallowed to keep the polling loop stable.</description>
///         </item>
///         <item>
///             <description>Consumers should handle user-facing error reporting at the application boundary.</description>
///         </item>
///     </list>
/// </remarks>
public sealed record MumbleService : IMumbleService, IDisposable
{
	private readonly IGw2Client _gw2Client;
	private readonly TimeSpan _pollInterval;
	private CancellationTokenSource? _cts;

	private volatile MumbleStateModel _current = new(
		false,
		new Coordinates3(),
		new Coordinates3(),
		0,
		"Not Available",
		DateTimeOffset.MinValue
	);

	/// <summary>
	///     Initializes a new instance of the <see cref="MumbleService" /> class.
	/// </summary>
	/// <param name="gw2Client">The GW2 client used to access the MumbleLink provider.</param>
	/// <param name="pollInterval">The polling interval. Defaults to 100 ms.</param>
	public MumbleService(IGw2Client gw2Client, TimeSpan? pollInterval = null)
	{
		_gw2Client = gw2Client ?? throw new ArgumentNullException(nameof(gw2Client));
		_pollInterval = pollInterval ?? TimeSpan.FromMilliseconds(100);
	}

	/// <summary>
	///     Stops polling and releases resources.
	/// </summary>
	public void Dispose()
	{
		Stop();

		if (_gw2Client is IDisposable d)
			try
			{
				d.Dispose();
			}
			catch
			{
				// Intentional no-op.
			}
	}

	/// <inheritdoc />
	public MumbleStateModel Current => _current;

	/// <inheritdoc />
	public event EventHandler<MumbleStateModel>? MumbleUpdated;

	/// <inheritdoc />
	public void Start()
	{
		if (_cts != null)
			return;

		_cts = new CancellationTokenSource();
		Task.Run(() => PollLoopAsync(_cts.Token), CancellationToken.None);
	}

	/// <inheritdoc />
	public void Stop()
	{
		var cts = _cts;
		if (cts == null)
			return;

		try
		{
			cts.Cancel();
		}
		finally
		{
			cts.Dispose();
			_cts = null;
		}
	}

	private async Task PollLoopAsync(CancellationToken cancellationToken)
	{
		while (!cancellationToken.IsCancellationRequested)
			try
			{
				_gw2Client.Mumble.Update();
				var available = _gw2Client.Mumble.IsAvailable;

				var newState = new MumbleStateModel(
					available,
					available ? _gw2Client.Mumble.AvatarPosition : new Coordinates3(),
					available ? _gw2Client.Mumble.CameraPosition : new Coordinates3(),
					available ? (uint)_gw2Client.Mumble.MapId : 0u,
					available ? _gw2Client.Mumble.CharacterName ?? string.Empty : "Not Available",
					DateTimeOffset.UtcNow
				);

				_current = newState;
				MumbleUpdated?.Invoke(this, newState);

				await Task.Delay(_pollInterval, cancellationToken).ConfigureAwait(false);
			}
			catch (TaskCanceledException)
			{
				break;
			}
			catch
			{
				// Swallow transient errors to keep polling stable.
				try
				{
					await Task.Delay(_pollInterval, cancellationToken).ConfigureAwait(false);
				}
				catch (TaskCanceledException)
				{
					break;
				}
			}
	}
}