namespace AI.SocialNetwork.Domain.Entities;

// План развития (результат метода Ветвей и границ)
public class DevelopmentPlan
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long? TargetRoleId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public User? User { get; set; }
    public ProfessionalRole? TargetRole { get; set; }
    public ICollection<PlanStep> Steps { get; set; } = new List<PlanStep>();
}

// Шаг плана (траектория Next Best Action)
public class PlanStep
{
    public long Id { get; set; }
    public long PlanId { get; set; }
    public long SkillId { get; set; }
    public int Order { get; set; }                   // из ветвей и границ
    public string? Material { get; set; }            // учебный материал / работа
    public decimal EstimatedHours { get; set; }
    public bool Completed { get; set; }

    public DevelopmentPlan? Plan { get; set; }
    public Skill? Skill { get; set; }
}