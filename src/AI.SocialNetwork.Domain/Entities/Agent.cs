namespace AI.SocialNetwork.Domain.Entities;

// AI-агент (дочерний объект владельца)
public class Agent
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string Name { get; set; } = string.Empty;        // "Иван AI"
    public string? SystemPrompt { get; set; }
    public string? CommunicationStyle { get; set; }
    public string LLMProvider { get; set; } = "ollama";     // "ollama", "openai"
    public string LLMModel { get; set; } = "nemotron-mini";
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    public User? User { get; set; }
    public ICollection<AgentMessage> Messages { get; set; } = new List<AgentMessage>();
    public ICollection<AgentActionLog> ActionLogs { get; set; } = new List<AgentActionLog>();
}