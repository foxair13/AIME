using AI.SocialNetwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AI.SocialNetwork.Infrastructure.Configuration;

// Модерация/безопасность: жалобы, отзывы, KYC, consent, аудит
public class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.ToTable("reports");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TargetType).HasMaxLength(30);
        builder.Property(x => x.Status).HasMaxLength(30);
        builder.HasIndex(x => new { x.Status, x.TargetType, x.TargetId });
    }
}

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("reviews");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Rating).HasDefaultValue(5);
        builder.HasOne(x => x.Author).WithMany().HasForeignKey(x => x.AuthorId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Target).WithMany().HasForeignKey(x => x.TargetId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.DealId, x.AuthorId }).IsUnique();
    }
}

public class KYCRequestConfiguration : IEntityTypeConfiguration<KYCRequest>
{
    public void Configure(EntityTypeBuilder<KYCRequest> builder)
    {
        builder.ToTable("kyc_requests");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Kind).HasMaxLength(30);
        builder.Property(x => x.Status).HasMaxLength(30);
        builder.HasIndex(x => new { x.UserId, x.Status });
    }
}

public class ConsentRecordConfiguration : IEntityTypeConfiguration<ConsentRecord>
{
    public void Configure(EntityTypeBuilder<ConsentRecord> builder)
    {
        builder.ToTable("consent_records");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Purpose).HasMaxLength(100);
        builder.HasIndex(x => new { x.UserId, x.Purpose }).IsUnique();
    }
}

public class AuditLogEntryConfiguration : IEntityTypeConfiguration<AuditLogEntry>
{
    public void Configure(EntityTypeBuilder<AuditLogEntry> builder)
    {
        builder.ToTable("audit_log_entries");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Action).HasMaxLength(100);
        builder.Property(x => x.EntityType).HasMaxLength(100);
        builder.HasIndex(x => new { x.CreatedAt, x.UserId });
    }
}

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_messages");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EventType).IsRequired().HasMaxLength(100);
        builder.Property(x => x.AggregateId).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Destination).HasMaxLength(300);
        builder.Property(x => x.Status).HasMaxLength(20).HasDefaultValue("pending");
        builder.HasIndex(x => new { x.Status, x.CreatedAt });
    }
}