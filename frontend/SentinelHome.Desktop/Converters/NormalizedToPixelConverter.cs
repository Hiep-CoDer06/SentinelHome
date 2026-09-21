using System.Globalization;
using System.Windows.Data;

namespace SentinelHome.Desktop.Converters;

/// <summary>Scales a 0..1 normalized coordinate to pixels for a frame dimension passed via ConverterParameter.</summary>
public sealed class NormalizedToPixelConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not double normalized)
            return 0d;

        var dimension = parameter switch
        {
            string text when double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed) => parsed,
            double d => d,
            _ => 0d
        };

        return normalized * dimension;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
