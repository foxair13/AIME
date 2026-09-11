namespace AI.SocialNetwork.Domain.Entities;

// Связи между пользователями
public class Connection
{
    public long Id { get; set; }
    public long UserFromId { get; set; }
    public long UserToId { get; set; }
    public string ConnectionType { get; set; } = string.Empty;  // "colleague", "client", "mentor"
    public decimal TrustScore { get; set; }                     // 0-100
    public DateTime CreatedAt { get; set; }

    public User? UserFrom { get; set; }
    public User? UserTo { get; set; }
}