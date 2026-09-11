namespace AI.SocialNetwork.Domain.Entities;

// Профессиональный стандарт / роль (агрегат весов навыков)
public class ProfessionalRole
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;           // "Senior C# Developer"
    public long? ParentRoleId { get; set; }                    // уровень выше
    public int Level { get; set; }                             // уровни квалификации 5/1, 6/1...

    public ProfessionalRole? ParentRole { get; set; }
    public ICollection<RoleSkillWeight> RequiredSkills { get; set; } = new List<RoleSkillWeight>();
}

// Вес навыка в роли
public class RoleSkillWeight
{
    public long RoleId { get; set; }
    public long SkillId { get; set; }
    public decimal Weight { get; set; }              // приоритет Vi, %
    public decimal MinScore { get; set; }            // порог допуска (в MVCDIS ~4/5)
    public decimal MaxScore { get; set; }            // максимум для нормировки (5)

    public ProfessionalRole? Role { get; set; }
    public Skill? Skill { get; set; }
}