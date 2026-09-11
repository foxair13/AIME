using AI.SocialNetwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AI.SocialNetwork.Infrastructure.Configuration;

// Канбан, файлы, коммуникации, уведомления
public class BoardConfiguration : IEntityTypeConfiguration<Board>
{
    public void Configure(EntityTypeBuilder<Board> builder)
    {
        builder.ToTable("boards");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
        builder.HasMany(x => x.Columns).WithOne(c => c.Board).HasForeignKey(c => c.BoardId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class BoardColumnConfiguration : IEntityTypeConfiguration<BoardColumn>
{
    public void Configure(EntityTypeBuilder<BoardColumn> builder)
    {
        builder.ToTable("board_columns");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.HasMany(x => x.Cards).WithOne(c => c.Column).HasForeignKey(c => c.ColumnId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class BoardCardConfiguration : IEntityTypeConfiguration<BoardCard>
{
    public void Configure(EntityTypeBuilder<BoardCard> builder)
    {
        builder.ToTable("board_cards");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).IsRequired().HasMaxLength(300);
        builder.Property(x => x.Status).HasMaxLength(20);
        builder.HasOne(x => x.RequiredSkill).WithMany().HasForeignKey(x => x.RequiredSkillId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.AssignedToUser).WithMany().HasForeignKey(x => x.AssignedToUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.ColumnId, x.Status });
    }
}

public class UserFileConfiguration : IEntityTypeConfiguration<UserFile>
{
    public void Configure(EntityTypeBuilder<UserFile> builder)
    {
        builder.ToTable("user_files");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(300);
        builder.Property(x => x.Kind).HasMaxLength(50);
    }
}

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.ToTable("conversations");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type).HasMaxLength(20);
        builder.Property(x => x.Title).HasMaxLength(200);
        builder.HasMany(x => x.Members).WithOne(m => m.Conversation).HasForeignKey(m => m.ConversationId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Messages).WithOne(m => m.Conversation).HasForeignKey(m => m.ConversationId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class ConversationMemberConfiguration : IEntityTypeConfiguration<ConversationMember>
{
    public void Configure(EntityTypeBuilder<ConversationMember> builder)
    {
        builder.ToTable("conversation_members");
        builder.HasKey(x => new { x.ConversationId, x.UserId });
        builder.Property(x => x.Role).HasMaxLength(30);
    }
}

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("messages");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Body).IsRequired();
        builder.HasIndex(x => new { x.ConversationId, x.CreatedAt });
    }
}

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("notifications");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type).HasMaxLength(50);
        builder.Property(x => x.Channel).HasMaxLength(20);
        builder.HasIndex(x => new { x.UserId, x.Read, x.CreatedAt });
    }
}