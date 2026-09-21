using System.Globalization;
using System.Windows.Data;

namespace SentinelHome.Desktop.Converters;

public sealed class PageEqualsConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value is string current && parameter is string expected
            && string.Equals(current, expected, StringComparison.Ordinal);

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
