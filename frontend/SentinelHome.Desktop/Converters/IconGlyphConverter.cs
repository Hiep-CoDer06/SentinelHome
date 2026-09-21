using System.Globalization;
using System.Windows.Data;
using SentinelHome.Desktop.Models;

namespace SentinelHome.Desktop.Converters;

/// <summary>Picks a consistent Segoe Fluent Icons glyph for a dashboard metric based on its tone.</summary>
public sealed class ToneIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value switch
        {
            StatusTone.Danger => "\uE7BA",
            StatusTone.Success => "\uE714",
            StatusTone.Warning => "\uE916",
            _ => "\uE787"
        };

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}

/// <summary>Picks a Segoe Fluent Icons glyph for a detected species name - keeps icon usage
/// consistent across dashboard, history and event-detail instead of hardcoding per record.</summary>
public sealed class SpeciesIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value is string species && species.Contains("Người", StringComparison.OrdinalIgnoreCase)
            ? "\uE77B"
            : "\uE8D0";

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
