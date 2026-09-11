using AI.SocialNetwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AI.SocialNetwork.Infrastructure.Configuration;

// Конфигурации Identity: User, Agent, Connection, портфолио, репутация
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(256);
        builder.HasIndex(x => x.Email).IsUnique();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Bio).HasMaxLength(2000);
        builder.Property(x => x.City).HasMaxLength(100);
        builder.Property(x => x.Country).HasMaxLength(100);
        builder.OwnsOne(x => x.Location, loc =>
        {
            loc.Property(l => l.Latitude).HasColumnName("latitude");
            loc.Property(l => l.Longitude).HasColumnName("longitude");
        });
        builder.HasMany(x => x.Agents).WithOne(a => a.User).HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.UserSkills).WithOne(us => us.User).HasForeignKey(us => us.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.ReputationEntries).WithOne(r => r.User).HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Portfolios).WithOne(p => p.User).HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.ConnectionsFrom).WithOne(c => c.UserFrom).HasForeignKey(c => c.UserFromId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.ConnectionsTo).WithOne(c => c.UserTo).HasForeignKey(c => c.UserToId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.AgentPermissions).WithOne(ap => ap.Owner).HasForeignKey(ap => ap.OwnerId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Consents).WithOne(c => c.User).HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Languages).WithOne(ul => ul.User).HasForeignKey(ul => ul.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class AgentConfiguration : IEntityTypeConfiguration<Agent>
{
    public void Configure(EntityTypeBuilder<Agent> builder)
    {
        builder.ToTable("agents");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.LLMProvider).HasMaxLength(50).HasDefaultValue("ollama");
        builder.Property(x => x.LLMModel).HasMaxLength(100).HasDefaultValue("nemotron-mini");
        builder.HasMany(x => x.Messages).WithOne(m => m.Agent).HasForeignKey(m => m.AgentId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.ActionLogs).WithOne(l => l.Agent).HasForeignKey(l => l.AgentId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class ConnectionConfiguration : IEntityTypeConfiguration<Connection>
{
    public void Configure(EntityTypeBuilder<Connection> builder)
    {
        builder.ToTable("connections");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ConnectionType).HasMaxLength(50);
        builder.Property(x => x.TrustScore).HasPrecision(6, 2);
        builder.HasIndex(x => new { x.UserFromId, x.UserToId }).IsUnique();
    }
}

public class PortfolioConfiguration : IEntityTypeConfiguration<Portfolio>
{
    public void Configure(EntityTypeBuilder<Portfolio> builder)
    {
        builder.ToTable("portfolios");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).IsRequired().HasMaxLength(300);
        builder.Property(x => x.Url).HasMaxLength(2000);
    }
}

public class ReputationEntryConfiguration : IEntityTypeConfiguration<ReputationEntry>
{
    public void Configure(EntityTypeBuilder<ReputationEntry> builder)
    {
        builder.ToTable("reputation_entries");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Value).HasPrecision(6, 2);
        builder.Property(x => x.Source).HasMaxLength(50);
        builder.HasIndex(x => new { x.UserId, x.SkillId });
    }
}

public class AgentMessageConfiguration : IEntityTypeConfiguration<AgentMessage>
{
    public void Configure(EntityTypeBuilder<AgentMessage> builder)
    {
        builder.ToTable("agent_messages");
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.TargetAgent).WithMany().HasForeignKey(x => x.TargetAgentId).OnDelete(DeleteBehavior.Restrict);
    }
}