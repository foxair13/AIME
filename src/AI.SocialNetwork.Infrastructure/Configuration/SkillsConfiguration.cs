using AI.SocialNetwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AI.SocialNetwork.Infrastructure.Configuration;

// Дерево навыков, роль/веса, аттестация, план развития
public class SkillConfiguration : IEntityTypeConfiguration<Skill>
{
    public void Configure(EntityTypeBuilder<Skill> builder)
    {
        builder.ToTable("skills");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.HasIndex(x => new { x.ParentId, x.Name });
        builder.HasMany(x => x.Children).WithOne(c => c.Parent).HasForeignKey(c => c.ParentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Category).WithMany(c => c.Skills).HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class SkillClosureConfiguration : IEntityTypeConfiguration<SkillClosure>
{
    public void Configure(EntityTypeBuilder<SkillClosure> builder)
    {
        builder.ToTable("skill_closure");
        builder.HasKey(x => new { x.AncestorId, x.DescendantId });
        builder.HasOne(x => x.Ancestor).WithMany().HasForeignKey(x => x.AncestorId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Descendant).WithMany().HasForeignKey(x => x.DescendantId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class UserSkillConfiguration : IEntityTypeConfiguration<UserSkill>
{
    public void Configure(EntityTypeBuilder<UserSkill> builder)
    {
        builder.ToTable("user_skills");
        builder.HasKey(x => new { x.UserId, x.SkillId });
        builder.Property(x => x.DecayRate).HasPrecision(10, 4);
        builder.Property(x => x.Level).HasDefaultValue(1);
    }
}

public class ProfessionalRoleConfiguration : IEntityTypeConfiguration<ProfessionalRole>
{
    public void Configure(EntityTypeBuilder<ProfessionalRole> builder)
    {
        builder.ToTable("professional_roles");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
        builder.HasMany(x => x.RequiredSkills).WithOne(r => r.Role).HasForeignKey(r => r.RoleId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class RoleSkillWeightConfiguration : IEntityTypeConfiguration<RoleSkillWeight>
{
    public void Configure(EntityTypeBuilder<RoleSkillWeight> builder)
    {
        builder.ToTable("role_skill_weights");
        builder.HasKey(x => new { x.RoleId, x.SkillId });
        builder.Property(x => x.Weight).HasPrecision(8, 4);
        builder.Property(x => x.MinScore).HasPrecision(6, 2);
        builder.Property(x => x.MaxScore).HasPrecision(6, 2);
    }
}

public class AttestationConfiguration : IEntityTypeConfiguration<Attestation>
{
    public void Configure(EntityTypeBuilder<Attestation> builder)
    {
        builder.ToTable("attestations");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Score).HasPrecision(6, 2);
        builder.HasOne(x => x.ExaminerUser).WithMany().HasForeignKey(x => x.ExaminerUserId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class DevelopmentPlanConfiguration : IEntityTypeConfiguration<DevelopmentPlan>
{
    public void Configure(EntityTypeBuilder<DevelopmentPlan> builder)
    {
        builder.ToTable("development_plans");
        builder.HasKey(x => x.Id);
        builder.HasMany(x => x.Steps).WithOne(s => s.Plan).HasForeignKey(s => s.PlanId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class PlanStepConfiguration : IEntityTypeConfiguration<PlanStep>
{
    public void Configure(EntityTypeBuilder<PlanStep> builder)
    {
        builder.ToTable("plan_steps");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Order).HasColumnName("order_no");
        builder.Property(x => x.EstimatedHours).HasPrecision(10, 2);
    }
}

public class SkillCategoryConfiguration : IEntityTypeConfiguration<SkillCategory>
{
    public void Configure(EntityTypeBuilder<SkillCategory> builder)
    {
        builder.ToTable("skill_categories");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Icon).HasMaxLength(100);
    }
}

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("organizations");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
    }
}