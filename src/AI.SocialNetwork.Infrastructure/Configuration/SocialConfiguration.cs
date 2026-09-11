using AI.SocialNetwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AI.SocialNetwork.Infrastructure.Configuration;

// Соцсеть: посты, группы, форумы, встречи, сделки
public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.ToTable("posts");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Body).IsRequired();
        builder.Property(x => x.ContentType).HasMaxLength(20);
        builder.Property(x => x.LanguageCode).HasMaxLength(10);
        builder.HasIndex(x => x.CreatedAt);
        builder.HasIndex(x => x.AuthorId);
        builder.HasMany(x => x.ContentLanguages).WithOne().HasForeignKey(x => x.ContentId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.ToTable("groups");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
        builder.HasMany(x => x.Permissions).WithOne(p => p.Group).HasForeignKey(p => p.GroupId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Members).WithOne(m => m.Group).HasForeignKey(m => m.GroupId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Posts).WithOne(p => p.Group).HasForeignKey(p => p.GroupId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class MembershipConfiguration : IEntityTypeConfiguration<Membership>
{
    public void Configure(EntityTypeBuilder<Membership> builder)
    {
        builder.ToTable("memberships");
        builder.HasKey(x => new { x.GroupId, x.UserId });
        builder.Property(x => x.Role).HasMaxLength(30).HasDefaultValue("member");
    }
}

public class GroupPermissionConfiguration : IEntityTypeConfiguration<GroupPermission>
{
    public void Configure(EntityTypeBuilder<GroupPermission> builder)
    {
        builder.ToTable("group_permissions");
        builder.HasKey(x => new { x.GroupId, x.RoleId, x.Action });
        builder.Property(x => x.RoleId).HasMaxLength(50);
        builder.Property(x => x.Action).HasMaxLength(50);
    }
}

public class ForumConfiguration : IEntityTypeConfiguration<Forum>
{
    public void Configure(EntityTypeBuilder<Forum> builder)
    {
        builder.ToTable("forums");
        builder.HasKey(x => x.Id);
        builder.HasMany(x => x.Categories).WithOne(c => c.Forum).HasForeignKey(c => c.ForumId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class ForumCategoryConfiguration : IEntityTypeConfiguration<ForumCategory>
{
    public void Configure(EntityTypeBuilder<ForumCategory> builder)
    {
        builder.ToTable("forum_categories");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
        builder.HasMany(x => x.Permissions).WithOne(p => p.Category).HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Topics).WithOne(t => t.Category).HasForeignKey(t => t.CategoryId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class ForumPermissionConfiguration : IEntityTypeConfiguration<ForumPermission>
{
    public void Configure(EntityTypeBuilder<ForumPermission> builder)
    {
        builder.ToTable("forum_permissions");
        builder.HasKey(x => new { x.CategoryId, x.RoleId, x.Action });
        builder.Property(x => x.RoleId).HasMaxLength(50);
        builder.Property(x => x.Action).HasMaxLength(50);
    }
}

public class ForumTopicConfiguration : IEntityTypeConfiguration<ForumTopic>
{
    public void Configure(EntityTypeBuilder<ForumTopic> builder)
    {
        builder.ToTable("forum_topics");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).IsRequired().HasMaxLength(250);
    }
}

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("subscriptions");
        builder.HasKey(x => new { x.UserId, x.TargetType, x.TargetUserId });
        builder.Property(x => x.TargetType).HasMaxLength(20);
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class MeetingConfiguration : IEntityTypeConfiguration<Meeting>
{
    public void Configure(EntityTypeBuilder<Meeting> builder)
    {
        builder.ToTable("meetings");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Status).HasMaxLength(20);
        builder.HasOne(x => x.UserA).WithMany().HasForeignKey(x => x.UserAId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.UserB).WithMany().HasForeignKey(x => x.UserBId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.UserAId, x.ProposedStart });
    }
}

public class DealConfiguration : IEntityTypeConfiguration<Deal>
{
    public void Configure(EntityTypeBuilder<Deal> builder)
    {
        builder.ToTable("deals");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Status).HasMaxLength(30);
        builder.Property(x => x.ContractTerms).HasMaxLength(10000);
        builder.Property(x => x.Amount).HasPrecision(14, 2);
        builder.Property(x => x.EscrowHeld).HasPrecision(14, 2);
        builder.HasOne(x => x.Client).WithMany().HasForeignKey(x => x.ClientId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Contractor).WithMany().HasForeignKey(x => x.ContractorId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Milestones).WithOne(m => m.Deal).HasForeignKey(m => m.DealId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => x.Status);
    }
}

public class DealMilestoneConfiguration : IEntityTypeConfiguration<DealMilestone>
{
    public void Configure(EntityTypeBuilder<DealMilestone> builder)
    {
        builder.ToTable("deal_milestones");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).IsRequired().HasMaxLength(300);
        builder.Property(x => x.Status).HasMaxLength(30);
        builder.Property(x => x.Amount).HasPrecision(14, 2);
    }
}

public class HashtagConfiguration : IEntityTypeConfiguration<Hashtag>
{
    public void Configure(EntityTypeBuilder<Hashtag> builder)
    {
        builder.ToTable("hashtags");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Tag).IsRequired().HasMaxLength(100);
        builder.HasIndex(x => x.Tag).IsUnique();
        builder.HasMany(x => x.Posts).WithOne().HasForeignKey(x => x.HashtagId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class PostHashtagConfiguration : IEntityTypeConfiguration<PostHashtag>
{
    public void Configure(EntityTypeBuilder<PostHashtag> builder)
    {
        builder.ToTable("post_hashtags");
        builder.HasKey(x => new { x.PostId, x.HashtagId });
        builder.HasOne(x => x.Post).WithMany(p => p.Hashtags).HasForeignKey(x => x.PostId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Hashtag).WithMany(h => h.Posts).HasForeignKey(x => x.HashtagId).OnDelete(DeleteBehavior.Cascade);
    }
}