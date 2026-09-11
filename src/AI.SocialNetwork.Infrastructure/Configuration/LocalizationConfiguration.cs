using AI.SocialNetwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AI.SocialNetwork.Infrastructure.Configuration;

// Локализация: языки, переводы, язык контента
public class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.ToTable("languages");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).IsRequired().HasMaxLength(10);
        builder.HasIndex(x => x.Code).IsUnique();
        builder.Property(x => x.NameNative).HasMaxLength(50);
        builder.Property(x => x.NameEn).HasMaxLength(50);
        builder.Property(x => x.IsRtl).HasDefaultValue(false);
        builder.Property(x => x.IsDefault).HasDefaultValue(false);
    }
}

public class UserLanguageConfiguration : IEntityTypeConfiguration<UserLanguage>
{
    public void Configure(EntityTypeBuilder<UserLanguage> builder)
    {
        builder.ToTable("user_languages");
        builder.HasKey(x => new { x.UserId, x.LanguageId });
        builder.Property(x => x.Level).HasMaxLength(20).HasDefaultValue("native");
        builder.HasOne(x => x.Language).WithMany().HasForeignKey(x => x.LanguageId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class TranslationConfiguration : IEntityTypeConfiguration<Translation>
{
    public void Configure(EntityTypeBuilder<Translation> builder)
    {
        builder.ToTable("translations");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Key).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Culture).IsRequired().HasMaxLength(10);
        builder.HasIndex(x => new { x.Key, x.Culture }).IsUnique();
        builder.Property(x => x.Context).HasMaxLength(100);
    }
}

public class ContentLanguageConfiguration : IEntityTypeConfiguration<ContentLanguage>
{
    public void Configure(EntityTypeBuilder<ContentLanguage> builder)
    {
        builder.ToTable("content_languages");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.LanguageCode).IsRequired().HasMaxLength(10);
        builder.HasIndex(x => new { x.ContentId, x.LanguageCode });
    }
}