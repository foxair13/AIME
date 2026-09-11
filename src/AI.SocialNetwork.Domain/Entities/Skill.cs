namespace AI.SocialNetwork.Domain.Entities;

// Дерево навыков (рекурсия через ParentId) + замыкающие таблицы
public class Skill
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public long? ParentId { get; set; }
    public long CategoryId { get; set; }
    public string Description { get; set; } = string.Empty;

    public Skill? Parent { get; set; }
    public ICollection<Skill> Children { get; set; } = new List<Skill>();
    public SkillCategory? Category { get; set; }
}

// Замыкающая таблица: быстрый обход предков/потомков (O(1))
public class SkillClosure
{
    public long AncestorId { get; set; }
    public long DescendantId { get; set; }
    public int Depth { get; set; }

    public Skill? Ancestor { get; set; }
    public Skill? Descendant { get; set; }
}