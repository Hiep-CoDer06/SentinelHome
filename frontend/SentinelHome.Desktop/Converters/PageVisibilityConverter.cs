using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SentinelHome.Desktop.Converters;

public sealed class PageVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value is string current && parameter is string expected && string.Equals(current, expected, StringComparison.Ordinal)
            ? Visibility.Visible
            : Visibility.Collapsed;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
