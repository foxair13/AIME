namespace AI.SocialNetwork.Domain.Entities;

// Категория навыков
public class SkillCategory
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;           // "IT", "Медицина", "Бизнес"
    public string? Icon { get; set; }

    public ICollection<Skill> Skills { get; set; } = new List<Skill>();
}