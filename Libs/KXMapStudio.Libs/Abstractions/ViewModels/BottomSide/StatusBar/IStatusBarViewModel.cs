namespace KXMapStudio.Libs.Abstractions.ViewModels.BottomSide.StatusBar;

/// <summary>
///     Exposes the bottom status bar state.
/// </summary>
/// <remarks>
///     This is a UI-facing abstraction (presentation layer). It must remain free of WPF visual types.
/// </remarks>
public interface IStatusBarViewModel
{
	MumbleConnectionState ConnectionState { get; }
	string CharacterName { get; }
	string MapText { get; }
	string CoordinatesText { get; }
	string XText { get; }
	string YText { get; }
	string ZText { get; }
}