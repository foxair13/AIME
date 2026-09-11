namespace AI.SocialNetwork.Domain.Events;

public class ConnectionCreatedEvent
{
    public long UserFromId { get; set; }
    public long UserToId { get; set; }
    public string ConnectionType { get; set; } = string.Empty;
}