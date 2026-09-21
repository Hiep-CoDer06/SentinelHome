using System.ComponentModel.DataAnnotations;

namespace SentinelHome.Contracts;

/// <summary>Payload received from the Task 1 computer-vision process.</summary>
public sealed record DetectionRequest
{
    [Required, MaxLength(100)] public required string CameraId { get; init; }
    /// <summary>Timestamp emitted by Task 1. A timezone-less value is interpreted as UTC+7.</summary>
    [Required] public required string CapturedAt { get; init; }
    [Required, MinLength(1)] public required IReadOnlyList<DetectedObjectRequest> Objects { get; init; }
    public EvidenceImageRequest? FullFrameImage { get; init; }
}

public sealed record DetectedObjectRequest
{
    [Required, MaxLength(200)] public required string SpeciesName { get; init; }
    [Range(0, 1)] public required float Confidence { get; init; }
    [Range(0, int.MaxValue)] public required int BBoxX { get; init; }
    [Range(0, int.MaxValue)] public required int BBoxY { get; init; }
    [Range(1, int.MaxValue)] public required int BBoxWidth { get; init; }
    [Range(1, int.MaxValue)] public required int BBoxHeight { get; init; }
}

public sealed record EvidenceImageRequest
{
    [Required, MaxLength(100)] public required string ContentType { get; init; }
    [Required] public required string Base64Data { get; init; }
}

/// <summary>API availability and server-clock information.</summary>
public sealed record HealthResponse(string Status, DateTimeOffset ServerTime);

/// <summary>Credentials submitted by a desktop user to obtain a JWT access token.</summary>
public sealed record LoginRequest(
    [property: Required, MaxLength(100)] string Username,
    [property: Required, MinLength(1)] string Password);

/// <summary>Authenticated desktop session information and its short-lived JWT access token.</summary>
public sealed record LoginResponse(string AccessToken, DateTimeOffset ExpiresAt, string Username, UserRole Role);

public sealed record EventSummaryResponse(
    Guid Id, DateTimeOffset CapturedAt, string PrimarySpeciesName, DangerLevel OverallDangerLevel,
    bool IsAcknowledged, bool WasAlerted, int DetectedObjectCount);

public sealed record AlertNotificationResponse(
    Guid EventId, DateTimeOffset CapturedAt, string PrimarySpeciesName,
    float Confidence, DangerLevel DangerLevel, int AdditionalDangerousObjects);
