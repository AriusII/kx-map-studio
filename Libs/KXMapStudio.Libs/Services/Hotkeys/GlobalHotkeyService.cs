using System.Runtime.InteropServices;

namespace KXMapStudio.Libs.Services.Hotkeys;

/// <summary>
///     Provides global hotkey registration using Windows API.
///     Implements F9 hotkey for adding markers from Mumble coordinates.
/// </summary>
/// <remarks>
///     This implementation uses low-level Windows API (RegisterHotKey/UnregisterHotKey)
///     to register system-wide hotkeys that work even when the application is in the background.
///     The service is defensive and handles registration failures gracefully.
/// </remarks>
public sealed class GlobalHotkeyService : IGlobalHotkeyService
{
	private const int WmHotkey = 0x0312;
	private const int HotkeyIdAddMarker = 1;

	// Virtual-Key Codes
	private const uint VkF9 = 0x78;

	// Modifiers
	private const uint ModNone = 0x0000;
	private bool _disposed;

	private nint _hwnd;
	private HwndSource? _hwndSource;
	private bool _isRegistered;

	/// <inheritdoc />
	public event EventHandler? AddMarkerFromMumblePressed;

	/// <summary>
	///     Initializes the hotkey service with a window handle for message processing.
	/// </summary>
	/// <param name="window">The WPF window to attach the hotkey handler to.</param>
	public void Initialize(Window window)
	{
		ArgumentNullException.ThrowIfNull(window);

		if (_hwndSource != null)
			return;

		var helper = new WindowInteropHelper(window);
		_hwnd = helper.Handle;

		if (_hwnd == nint.Zero)
		{
			// Window not yet shown, hook into SourceInitialized
			window.SourceInitialized += (_, _) =>
			{
				_hwnd = new WindowInteropHelper(window).Handle;
				_hwndSource = HwndSource.FromHwnd(_hwnd);
				_hwndSource?.AddHook(WndProc);
				RegisterHotkeys();
			};
		}
		else
		{
			_hwndSource = HwndSource.FromHwnd(_hwnd);
			_hwndSource?.AddHook(WndProc);
		}
	}

	/// <inheritdoc />
	public void RegisterHotkeys()
	{
		if (_hwnd == nint.Zero || _isRegistered)
			return;

		try
		{
			var registered = RegisterHotKey(_hwnd, HotkeyIdAddMarker, ModNone, VkF9);
			if (registered)
			{
				_isRegistered = true;
				Debug.WriteLine("[GlobalHotkeyService] F9 hotkey registered successfully.");
			}
			else
			{
				Debug.WriteLine(
					"[GlobalHotkeyService] Failed to register F9 hotkey (may be in use by another application).");
			}
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"[GlobalHotkeyService] Exception during hotkey registration: {ex.Message}");
		}
	}

	/// <inheritdoc />
	public void UnregisterHotkeys()
	{
		if (_hwnd == nint.Zero || !_isRegistered)
			return;

		try
		{
			UnregisterHotKey(_hwnd, HotkeyIdAddMarker);
			_isRegistered = false;
			Debug.WriteLine("[GlobalHotkeyService] F9 hotkey unregistered.");
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"[GlobalHotkeyService] Exception during hotkey unregistration: {ex.Message}");
		}
	}

	public void Dispose()
	{
		if (_disposed)
			return;

		UnregisterHotkeys();

		if (_hwndSource != null)
		{
			_hwndSource.RemoveHook(WndProc);
			_hwndSource = null;
		}

		_disposed = true;
	}

	private nint WndProc(nint hwnd, int msg, nint wParam, nint lParam, ref bool handled)
	{
		if (msg == WmHotkey)
		{
			var hotkeyId = wParam.ToInt32();
			if (hotkeyId == HotkeyIdAddMarker)
			{
				AddMarkerFromMumblePressed?.Invoke(this, EventArgs.Empty);
				handled = true;
			}
		}

		return nint.Zero;
	}

	#region Windows API

	[DllImport("user32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool RegisterHotKey(nint hWnd, int id, uint fsModifiers, uint vk);

	[DllImport("user32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool UnregisterHotKey(nint hWnd, int id);

	#endregion
}