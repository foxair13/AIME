namespace AI.SocialNetwork.Domain.Entities;

// Жалоба на контент/аккаунт (аудит: модерация)
public class Report
{
    public long Id { get; set; }
    public long ReporterId { get; set; }
    public string TargetType { get; set; } = string.Empty;  // post | user | group | message
    public long TargetId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Details { get; set; }
    public string Status { get; set; } = "new";            // new | under_review | actioned | dismissed
    public DateTime CreatedAt { get; set; }

    public User? Reporter { get; set; }
}

// Отзыв о контрагенте/сделке
public class Review
{
    public long Id { get; set; }
    public long DealId { get; set; }
    public long AuthorId { get; set; }
    public long TargetId { get; set; }
    public int Rating { get; set; }                        // 1..5
    public string? Comment { get; set; }
    public bool VerifiedPurchase { get; set; }
    public DateTime CreatedAt { get; set; }

    public User? Author { get; set; }
    public User? Target { get; set; }
}

// KYC / верификация личности (аудит: безопасность)
public class KYCRequest
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string Kind { get; set; } = string.Empty;       // phone | email | id_passport | liveness
    public string Status { get; set; } = "pending";        // pending | approved | rejected
    public string? ProviderRef { get; set; }
    public string? DocumentsJson { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }

    public User? User { get; set; }
}

// Согласие пользователя (GDPR — аудит)
public class ConsentRecord
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string Purpose { get; set; } = string.Empty;    // marketing | analytics | data_sharing
    public bool Granted { get; set; }
    public DateTime GrantedAt { get; set; }
    public DateTime? RevokedAt { get; set; }

    public User? User { get; set; }
}

// Неизменяемый журнал аудита (append-only)
public class AuditLogEntry
{
    public long Id { get; set; }
    public long? UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public long EntityId { get; set; }
    public string? DetailsJson { get; set; }
    public string? IpAddress { get; set; }
    public DateTime CreatedAt { get; set; }
}

// Transactional Outbox: событие для внешней доставки (Kafka/webhooks/push/email)
public class OutboxMessage
{
    public long Id { get; set; }
    public string EventType { get; set; } = string.Empty;   // notification.created | deal.completed | ...
    public string AggregateId { get; set; } = string.Empty;
    public string PayloadJson { get; set; } = string.Empty;
    public string? Destination { get; set; }                // kafka.topic | webhook.url | push | email
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public int RetryCount { get; set; }
    public string Status { get; set; } = "pending";         // pending | delivered | failed
    public string? LastError { get; set; }
}