using System.Globalization;
using System.Windows;
using System.Windows.Data;
using SentinelHome.Desktop.Models;

namespace SentinelHome.Desktop.Converters;

/// <summary>
/// Maps a <see cref="StatusTone"/> (or a Low/Medium/High severity string) to a themed brush.
/// ConverterParameter selects the variant: "Solid" (default), "Soft", or "Foreground".
/// </summary>
public sealed class ToneBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var tone = value switch
        {
            StatusTone t => t,
            "High" => StatusTone.Danger,
            "Medium" => StatusTone.Warning,
            "Low" => StatusTone.Info,
            _ => StatusTone.Info
        };

        var variant = parameter as string ?? "Solid";
        var key = (tone, variant) switch
        {
            (StatusTone.Danger, "Soft") => "DangerSoftBrush",
            (StatusTone.Warning, "Soft") => "WarningSoftBrush",
            (StatusTone.Success, "Soft") => "AccentSoftBrush",
            (StatusTone.Info, "Soft") => "InfoSoftBrush",
            (StatusTone.Danger, _) => "DangerBrush",
            (StatusTone.Warning, _) => "WarningBrush",
            (StatusTone.Success, _) => "AccentBrush",
            _ => "InfoBrush"
        };

        return Application.Current.TryFindResource(key) ?? Application.Current.TryFindResource("InfoBrush")!;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
