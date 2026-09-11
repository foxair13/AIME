namespace AI.SocialNetwork.Domain.Events;

public class AgentMessageSentEvent
{
    public long AgentId { get; set; }
    public long? TargetAgentId { get; set; }
    public string Content { get; set; } = string.Empty;
}