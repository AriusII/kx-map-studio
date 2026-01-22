namespace KXMapStudio.Libs.Converters;

/// <summary>
///     Converts a null or empty string to Visibility.
///     Null or empty string -> Collapsed, otherwise -> Visible.
/// </summary>
public sealed class NullOrEmptyToVisibilityConverter : IValueConverter
{
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return string.IsNullOrWhiteSpace(value as string) ? Visibility.Collapsed : Visibility.Visible;
	}

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotSupportedException();
	}
}