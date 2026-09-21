namespace SentinelHome.Desktop.Models;

/// <summary>Tone drives which semantic brush (info/success/warning/danger) a card or badge uses.</summary>
public enum StatusTone
{
    Info,
    Success,
    Warning,
    Danger
}

public sealed record DashboardMetric(string Icon, string Label, string Value, string Detail, StatusTone Tone);

/// <summary>One object detected inside a single DetectionEvent frame (mirrors DetectedObjects table).</summary>
public sealed record DetectedObjectInfo(string Species, string Confidence, bool IsDangerMatch);

public sealed record DashboardAlert(
    string Id,
    string Severity,
    string Species,
    string Confidence,
    string CapturedAt,
    string Camera,
    string Description,
    string ThumbnailGlyph,
    IReadOnlyList<DetectedObjectInfo> Objects,
    IReadOnlyList<DetectionOverlay> Overlays,
    bool IsDuplicate = false,
    string? OriginalEventTime = null)
{
    public string SeverityLabel => Severity switch
    {
        "High" => "NGHIÊM TRỌNG",
        "Medium" => "CẦN CHÚ Ý",
        _ => "THÔNG TIN"
    };

    public StatusTone SeverityTone => Severity switch
    {
        "High" => StatusTone.Danger,
        "Medium" => StatusTone.Warning,
        _ => StatusTone.Info
    };

    public string OtherObjectsSummary => Objects.Count > 1
        ? $"+{Objects.Count - 1} loài khác trong khung hình"
        : string.Empty;
}

public sealed record RecentDetection(
    string Time,
    string Species,
    string Camera,
    string Confidence,
    string Severity,
    string Status,
    string Date,
    string Icon);

/// <summary>
/// Bounding box của Task 1 (Computer Vision) được biểu diễn dạng normalized coordinate
/// (0..1 theo chiều rộng/cao khung hình gốc) — đúng chuẩn dữ liệu AI thường trả về và độc lập
/// với kích thước hiển thị. UI quy đổi sang pixel tại thời điểm vẽ bằng
/// <see cref="ToPixelRect"/>, nên khi thay khung video demo bằng khung API/video thật với
/// kích thước khác, lớp bounding-box vẫn khớp chính xác mà không cần đổi dữ liệu.
/// </summary>
public sealed record DetectionOverlay(
    string Label,
    string Confidence,
    string Severity,
    double NormalizedLeft,
    double NormalizedTop,
    double NormalizedWidth,
    double NormalizedHeight)
{
    public (double Left, double Top, double Width, double Height) ToPixelRect(double frameWidth, double frameHeight) =>
        (NormalizedLeft * frameWidth, NormalizedTop * frameHeight, NormalizedWidth * frameWidth, NormalizedHeight * frameHeight);
}
