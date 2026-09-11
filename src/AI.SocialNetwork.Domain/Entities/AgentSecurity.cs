namespace AI.SocialNetwork.Domain.Entities;

// Webhook-подписка на события (аудит: открытость)
public class WebhookSubscription
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string EventTypes { get; set; } = string.Empty;   // comma-separated
    public string? Secret { get; set; }
    public bool Active { get; set; }
}

// Право AI-агента от имени владельца (аудит: безопасность)
public class AgentPermission
{
    public long Id { get; set; }
    public long OwnerId { get; set; }
    public string Action { get; set; } = string.Empty;       // send_message|create_deal|pay|book_meeting...
    public bool Allowed { get; set; }
    public decimal? DailyLimitAmount { get; set; }
    public int? DailyLimitCount { get; set; }

    public User? Owner { get; set; }
}

// Лог решения агента (объяснимость)
public class AgentActionLog
{
    public long Id { get; set; }
    public long AgentId { get; set; }
    public long OwnerId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? InputJson { get; set; }
    public string? Explanation { get; set; }
    public bool Allowed { get; set; }
    public DateTime CreatedAt { get; set; }

    public Agent? Agent { get; set; }
}

// Событие аналитики (аудит: аналитика профиля)
public class AnalyticsEvent
{
    public long Id { get; set; }
    public long? UserId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string? PropertiesJson { get; set; }
    public DateTime CreatedAt { get; set; }
}