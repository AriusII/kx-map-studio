using KXMapStudio.Core.Models.Mumble;

namespace KXMapStudio.Libs.ViewModels.BottomSide.StatusBar;

public sealed partial class StatusBarViewModel : ObservableObject, IStatusBarViewModel, IDisposable
{
	private readonly IMumbleService _mumbleService;

	[ObservableProperty] private string _characterName = "Not connected";
	[ObservableProperty] private MumbleConnectionState _connectionState = MumbleConnectionState.Disconnected;
	[ObservableProperty] private string _coordinatesText = "Pos: N/A";
	[ObservableProperty] private string _mapText = "Map: N/A";
	[ObservableProperty] private string _xText = "X: -";
	[ObservableProperty] private string _yText = "Y: -";
	[ObservableProperty] private string _zText = "Z: -";

	public StatusBarViewModel(IMumbleService mumbleService)
	{
		_mumbleService = mumbleService;
		_mumbleService.MumbleUpdated += OnMumbleUpdated;
	}

	public void Dispose()
	{
		_mumbleService.MumbleUpdated -= OnMumbleUpdated;
	}

	[RelayCommand]
	private void OpenWebsite()
	{
		try
		{
			Process.Start(new ProcessStartInfo
				{ FileName = Constants.Settings.KxToolsWebsiteUrl, UseShellExecute = true });
		}
		catch
		{
		}
	}

	[RelayCommand]
	private void OpenDiscord()
	{
		try
		{
			Process.Start(new ProcessStartInfo
				{ FileName = Constants.Settings.DiscordInviteUrl, UseShellExecute = true });
		}
		catch
		{
		}
	}

	[RelayCommand]
	private void OpenGitHub()
	{
		try
		{
			Process.Start(new ProcessStartInfo { FileName = Constants.Settings.GitHubRepoUrl, UseShellExecute = true });
		}
		catch
		{
		}
	}

	private void OnMumbleUpdated(object? sender, MumbleStateModel mumble)
	{
		Application.Current?.Dispatcher.Invoke((Action)(() =>
		{
			ConnectionState = mumble.ConnectionState;

			if (mumble.ConnectionState == MumbleConnectionState.Disconnected)
			{
				CharacterName = "Not connected";
				MapText = "Map: N/A";
				CoordinatesText = "Pos: N/A";
				XText = "X: -";
				YText = "Y: -";
				ZText = "Z: -";
				return;
			}

			if (mumble.ConnectionState == MumbleConnectionState.Stale)
				CharacterName = string.IsNullOrWhiteSpace(mumble.CharacterName)
					? "Unknown"
					: $"{mumble.CharacterName} (AFK)";
			else
				CharacterName = string.IsNullOrWhiteSpace(mumble.CharacterName) ? "Unknown" : mumble.CharacterName;

			MapText = $"Map: {mumble.CurrentMapId}";

			var x = mumble.PlayerPosition.X;
			var y = mumble.PlayerPosition.Y;
			var z = mumble.PlayerPosition.Z;

			XText = $"X: {x:0.##}";
			YText = $"Y: {y:0.##}";
			ZText = $"Z: {z:0.##}";
			CoordinatesText = $"Pos: {x:0.##}, {y:0.##}, {z:0.##}";
		}));
	}
}