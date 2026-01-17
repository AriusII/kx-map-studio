namespace KXMapStudio.Libs.Converters;

public sealed class InverseCountToVisibilityConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		var count = value switch
		{
			int i => i,
			ICollection c => c.Count,
			null => 0,
			_ => 0
		};

		return count == 0 ? Visibility.Visible : Visibility.Collapsed;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}