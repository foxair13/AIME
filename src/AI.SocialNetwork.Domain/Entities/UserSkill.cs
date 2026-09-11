namespace AI.SocialNetwork.Domain.Entities;

// Связь пользователь-навык (динамика во времени)
public class UserSkill
{
    public long UserId { get; set; }
    public long SkillId { get; set; }
    public int Level { get; set; }                  // 1-10
    public bool Verified { get; set; }
    public DateTime LastConfirmedAt { get; set; }   // для коэффициента устаревания
    public decimal DecayRate { get; set; }          // k из формулы (3.29)

    public User? User { get; set; }
    public Skill? Skill { get; set; }
}