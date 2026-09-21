using SentinelHome.Contracts;

namespace SentinelHome.Data;

public sealed class Species
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? DisplayNameVi { get; set; }
    public DangerLevel DangerLevel { get; set; }
    public float ConfidenceThreshold { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<DetectedObject> DetectedObjects { get; } = new List<DetectedObject>();
}

public sealed class DetectionEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string CameraId { get; set; }
    public DateTimeOffset CapturedAt { get; set; }
    public DateTimeOffset ReceivedAt { get; set; }
    public DangerLevel OverallDangerLevel { get; set; }
    public bool IsDuplicate { get; set; }
    public Guid? OriginalEventId { get; set; }
    public DetectionEvent? OriginalEvent { get; set; }
    public ICollection<DetectionEvent> DuplicateEvents { get; } = new List<DetectionEvent>();
    public bool WasAlerted { get; set; }
    public DateTimeOffset? AlertedAt { get; set; }
    public bool IsAcknowledged { get; set; }
    public DateTimeOffset? AcknowledgedAt { get; set; }
    public ICollection<DetectedObject> DetectedObjects { get; } = new List<DetectedObject>();
    public ICollection<EvidenceImage> EvidenceImages { get; } = new List<EvidenceImage>();
}

public sealed class DetectedObject
{
    public long Id { get; set; }
    public Guid DetectionEventId { get; set; }
    public DetectionEvent DetectionEvent { get; set; } = null!;
    public int? SpeciesId { get; set; }
    public Species? Species { get; set; }
    public required string SpeciesNameRaw { get; set; }
    public float Confidence { get; set; }
    public int BBoxX { get; set; }
    public int BBoxY { get; set; }
    public int BBoxWidth { get; set; }
    public int BBoxHeight { get; set; }
    public bool IsDangerMatch { get; set; }
}

public sealed class EvidenceImage
{
    public long Id { get; set; }
    public Guid DetectionEventId { get; set; }
    public DetectionEvent DetectionEvent { get; set; } = null!;
    public EvidenceImageType ImageType { get; set; }
    public required byte[] ImageData { get; set; }
    public required string ContentType { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public sealed class AlertSettings
{
    public int Id { get; set; } = 1;
    public bool IsEnabled { get; set; } = true;
    public DangerLevel MinDangerLevelToTrigger { get; set; } = DangerLevel.Medium;
    public bool IsSoundEnabled { get; set; } = true;
    public int DebounceWindowSeconds { get; set; } = 5;
}

public sealed class User
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string PasswordHash { get; set; }
    public UserRole Role { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? LastLoginAt { get; set; }
}

public sealed class SystemLog
{
    public long Id { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public SystemLogLevel Level { get; set; }
    public required string Source { get; set; }
    public required string Message { get; set; }
    public string? ExceptionDetails { get; set; }
}
