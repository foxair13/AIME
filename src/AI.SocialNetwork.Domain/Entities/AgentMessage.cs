namespace AI.SocialNetwork.Domain.Entities;

// Сообщения агентов
public class AgentMessage
{
    public long Id { get; set; }
    public long AgentId { get; set; }
    public long? TargetAgentId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public bool IsRead { get; set; }

    public Agent? Agent { get; set; }
    public Agent? TargetAgent { get; set; }
}