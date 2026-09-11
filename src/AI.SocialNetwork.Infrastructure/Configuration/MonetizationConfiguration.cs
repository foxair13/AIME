using AI.SocialNetwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AI.SocialNetwork.Infrastructure.Configuration;

// Деньги: планы подписок, донаты, инвойсы, рефанды, рефералки
public class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
    {
        builder.ToTable("subscription_plans");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Currency).HasMaxLength(10).HasDefaultValue("USD");
        builder.Property(x => x.PriceMonthly).HasPrecision(10, 2);
        builder.HasMany(x => x.Subscribers).WithOne(s => s.Plan).HasForeignKey(s => s.PlanId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class PlanSubscriberConfiguration : IEntityTypeConfiguration<PlanSubscriber>
{
    public void Configure(EntityTypeBuilder<PlanSubscriber> builder)
    {
        builder.ToTable("plan_subscribers");
        builder.HasKey(x => new { x.PlanId, x.UserId });
    }
}

public class DonationConfiguration : IEntityTypeConfiguration<Donation>
{
    public void Configure(EntityTypeBuilder<Donation> builder)
    {
        builder.ToTable("donations");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Currency).HasMaxLength(10);
        builder.Property(x => x.Amount).HasPrecision(14, 2);
        builder.HasOne(x => x.From).WithMany().HasForeignKey(x => x.FromId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.To).WithMany().HasForeignKey(x => x.ToId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class ReferralLinkConfiguration : IEntityTypeConfiguration<ReferralLink>
{
    public void Configure(EntityTypeBuilder<ReferralLink> builder)
    {
        builder.ToTable("referral_links");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
        builder.HasIndex(x => x.Code).IsUnique();
    }
}

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("invoices");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Number).HasMaxLength(100);
        builder.Property(x => x.Status).HasMaxLength(30);
        builder.Property(x => x.Total).HasPrecision(14, 2);
        builder.Property(x => x.PlatformFee).HasPrecision(14, 2);
        builder.Property(x => x.TaxRate).HasPrecision(6, 2);
    }
}

public class RefundRequestConfiguration : IEntityTypeConfiguration<RefundRequest>
{
    public void Configure(EntityTypeBuilder<RefundRequest> builder)
    {
        builder.ToTable("refund_requests");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Status).HasMaxLength(30);
        builder.Property(x => x.Amount).HasPrecision(14, 2);
    }
}

public class WebhookSubscriptionConfiguration : IEntityTypeConfiguration<WebhookSubscription>
{
    public void Configure(EntityTypeBuilder<WebhookSubscription> builder)
    {
        builder.ToTable("webhook_subscriptions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Url).HasMaxLength(2000);
        builder.HasIndex(x => x.Url);
    }
}

public class AgentPermissionConfiguration : IEntityTypeConfiguration<AgentPermission>
{
    public void Configure(EntityTypeBuilder<AgentPermission> builder)
    {
        builder.ToTable("agent_permissions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Action).HasMaxLength(100);
        builder.Property(x => x.DailyLimitAmount).HasPrecision(14, 2);
        builder.HasIndex(x => new { x.OwnerId, x.Action }).IsUnique();
    }
}

public class AgentActionLogConfiguration : IEntityTypeConfiguration<AgentActionLog>
{
    public void Configure(EntityTypeBuilder<AgentActionLog> builder)
    {
        builder.ToTable("agent_action_logs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Action).HasMaxLength(100);
        builder.Property(x => x.Explanation).HasMaxLength(4000);
        builder.HasIndex(x => new { x.AgentId, x.CreatedAt });
    }
}

public class AnalyticsEventConfiguration : IEntityTypeConfiguration<AnalyticsEvent>
{
    public void Configure(EntityTypeBuilder<AnalyticsEvent> builder)
    {
        builder.ToTable("analytics_events");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EventType).HasMaxLength(100);
        builder.HasIndex(x => new { x.EventType, x.CreatedAt });
    }
}