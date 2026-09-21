using Microsoft.EntityFrameworkCore;

namespace SentinelHome.Data;

public sealed class SentinelHomeDbContext(DbContextOptions<SentinelHomeDbContext> options) : DbContext(options)
{
    public DbSet<Species> Species => Set<Species>();
    public DbSet<DetectionEvent> DetectionEvents => Set<DetectionEvent>();
    public DbSet<DetectedObject> DetectedObjects => Set<DetectedObject>();
    public DbSet<EvidenceImage> EvidenceImages => Set<EvidenceImage>();
    public DbSet<AlertSettings> AlertSettings => Set<AlertSettings>();
    public DbSet<User> Users => Set<User>();
    public DbSet<SystemLog> SystemLogs => Set<SystemLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Species>(entity =>
        {
            entity.HasIndex(x => x.Name).IsUnique();
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.DisplayNameVi).HasMaxLength(200);
            entity.Property(x => x.DangerLevel).HasConversion<string>().HasMaxLength(16);
            entity.ToTable(t => t.HasCheckConstraint("ck_species_confidence", "\"ConfidenceThreshold\" >= 0 AND \"ConfidenceThreshold\" <= 1"));
        });

        modelBuilder.Entity<DetectionEvent>(entity =>
        {
            entity.Property(x => x.CameraId).HasMaxLength(100).IsRequired();
            entity.Property(x => x.OverallDangerLevel).HasConversion<string>().HasMaxLength(16);
            entity.HasIndex(x => new { x.WasAlerted, x.IsAcknowledged, x.CapturedAt });
            entity.HasIndex(x => new { x.CameraId, x.CapturedAt });
            entity.HasOne(x => x.OriginalEvent).WithMany(x => x.DuplicateEvents)
                .HasForeignKey(x => x.OriginalEventId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<DetectedObject>(entity =>
        {
            entity.Property(x => x.SpeciesNameRaw).HasMaxLength(200).IsRequired();
            entity.HasIndex(x => new { x.DetectionEventId, x.IsDangerMatch });
            entity.HasOne(x => x.DetectionEvent).WithMany(x => x.DetectedObjects)
                .HasForeignKey(x => x.DetectionEventId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Species).WithMany(x => x.DetectedObjects)
                .HasForeignKey(x => x.SpeciesId).OnDelete(DeleteBehavior.SetNull);
            entity.ToTable(t =>
            {
                t.HasCheckConstraint("ck_object_confidence", "\"Confidence\" >= 0 AND \"Confidence\" <= 1");
                t.HasCheckConstraint("ck_object_bbox", "\"BBoxWidth\" > 0 AND \"BBoxHeight\" > 0");
            });
        });

        modelBuilder.Entity<EvidenceImage>(entity =>
        {
            entity.Property(x => x.ImageType).HasConversion<string>().HasMaxLength(16);
            entity.Property(x => x.ContentType).HasMaxLength(100).IsRequired();
            entity.HasOne(x => x.DetectionEvent).WithMany(x => x.EvidenceImages)
                .HasForeignKey(x => x.DetectionEventId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AlertSettings>(entity =>
        {
            entity.Property(x => x.MinDangerLevelToTrigger).HasConversion<string>().HasMaxLength(16);
            entity.ToTable(t => t.HasCheckConstraint("ck_alert_settings_singleton", "\"Id\" = 1"));
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(x => x.Username).IsUnique();
            entity.Property(x => x.Username).HasMaxLength(100).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(255).IsRequired();
            entity.Property(x => x.Role).HasConversion<string>().HasMaxLength(16);
        });

        modelBuilder.Entity<SystemLog>(entity =>
        {
            entity.Property(x => x.Level).HasConversion<string>().HasMaxLength(16);
            entity.Property(x => x.Source).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Message).HasMaxLength(2000).IsRequired();
            entity.HasIndex(x => new { x.Timestamp, x.Level });
        });
    }
}
