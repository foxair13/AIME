namespace AI.SocialNetwork.Domain.Entities;

// Репутация (временные серии)
public class ReputationEntry
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long SkillId { get; set; }
    public decimal Value { get; set; }              // 0-100
    public string Source { get; set; } = string.Empty;  // "project", "test", "peer"
    public long? ProjectId { get; set; }
    public DateTime CreatedAt { get; set; }

    public User? User { get; set; }
    public Skill? Skill { get; set; }
}