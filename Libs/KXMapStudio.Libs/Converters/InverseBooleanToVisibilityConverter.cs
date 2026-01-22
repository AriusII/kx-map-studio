namespace KXMapStudio.Libs.Converters;

/// <summary>
///     Converts a boolean value to a Visibility value (inverted: true -> Collapsed, false -> Visible).
/// </summary>
public sealed class InverseBooleanToVisibilityConverter : IValueConverter
{
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return value is true ? Visibility.Collapsed : Visibility.Visible;
	}

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return value is Visibility.Collapsed;
	}
}