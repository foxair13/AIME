namespace AI.SocialNetwork.Domain.Entities;

// Лента: подписки и timeline
public class Subscription
{
    public long UserId { get; set; }
    public long TargetUserId { get; set; }   // / GroupId / ForumId / SkillId
    public string TargetType { get; set; } = "user";   // "user" | "group" | "forum" | "skill"
    public DateTime CreatedAt { get; set; }

    public User? User { get; set; }
}

// Встреча (календарь)
public class Meeting
{
    public long Id { get; set; }
    public long UserAId { get; set; }
    public long UserBId { get; set; }
    public DateTimeOffset ProposedStart { get; set; }
    public DateTimeOffset? ConfirmedAt { get; set; }
    public string Status { get; set; } = "proposed";   // proposed | confirmed | declined
    public long? DealId { get; set; }

    public User? UserA { get; set; }
    public User? UserB { get; set; }
}

// Сделка (escrow)
public class Deal
{
    public long Id { get; set; }
    public long ClientId { get; set; }
    public long ContractorId { get; set; }
    public long? TaskId { get; set; }        // привязка к Kanban-карточке
    public decimal Amount { get; set; }
    public decimal EscrowHeld { get; set; }
    public string Status { get; set; } = "created";   // created/escrowed/in_progress/delivered/completed/disputed
    public string? ContractTerms { get; set; }        // контракт/ТЗ
    public long? PaymentId { get; set; }

    public User? Client { get; set; }
    public User? Contractor { get; set; }
    public ICollection<DealMilestone> Milestones { get; set; } = new List<DealMilestone>();
}

// Веха сделки (escrow по частям) — аудит
public class DealMilestone
{
    public long Id { get; set; }
    public long DealId { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = "pending";   // pending | completed | released
    public DateTime? CompletedAt { get; set; }

    public Deal? Deal { get; set; }
}