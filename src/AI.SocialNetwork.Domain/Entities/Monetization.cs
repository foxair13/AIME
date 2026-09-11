namespace AI.SocialNetwork.Domain.Entities;

// Платная подписка на автора/контент (аудит: монетизация)
public class SubscriptionPlan
{
    public long Id { get; set; }
    public long? AuthorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal PriceMonthly { get; set; }
    public string Currency { get; set; } = "USD";

    public User? Author { get; set; }
    public ICollection<PlanSubscriber> Subscribers { get; set; } = new List<PlanSubscriber>();
}

public class PlanSubscriber
{
    public long PlanId { get; set; }
    public long UserId { get; set; }
    public DateTime SubscribedAt { get; set; }
    public DateTime? CancelledAt { get; set; }

    public SubscriptionPlan? Plan { get; set; }
    public User? User { get; set; }
}

// Донат
public class Donation
{
    public long Id { get; set; }
    public long FromId { get; set; }
    public long ToId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime CreatedAt { get; set; }

    public User? From { get; set; }
    public User? To { get; set; }
}

// Реферальная ссылка (аудит: виральность)
public class ReferralLink
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string Code { get; set; } = string.Empty;

    public User? User { get; set; }
}

// Инвойс (аудит: налоги/деньги)
public class Invoice
{
    public long Id { get; set; }
    public long? DealId { get; set; }
    public string Number { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public decimal PlatformFee { get; set; }
    public decimal TaxRate { get; set; }
    public string Status { get; set; } = "draft";      // draft/sent/paid/cancelled
    public DateTime? PaidAt { get; set; }
}

// Запрос возврата средств
public class RefundRequest
{
    public long Id { get; set; }
    public long DealId { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = "pending";    // pending | approved | rejected | refunded
    public bool Refunded { get; set; }
}