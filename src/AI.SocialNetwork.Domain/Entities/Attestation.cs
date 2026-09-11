namespace AI.SocialNetwork.Domain.Entities;

// Аттестация / верификация навыка
public class Attestation
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long SkillId { get; set; }
    public long? ExaminerUserId { get; set; }       // эксперт
    public string Type { get; set; } = string.Empty;   // "quiz", "expert", "peer"
    public decimal Score { get; set; }              // 0..100
    public bool Passed { get; set; }
    public DateTime CreatedAt { get; set; }

    public User? User { get; set; }
    public Skill? Skill { get; set; }
    public User? ExaminerUser { get; set; }
}