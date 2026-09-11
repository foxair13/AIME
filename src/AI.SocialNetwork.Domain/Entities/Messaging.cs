namespace AI.SocialNetwork.Domain.Entities;

// Диалог/групповой чат (аудит: коммуникации)
public class Conversation
{
    public long Id { get; set; }
    public string Type { get; set; } = "dm";          // dm | group | channel
    public string? Title { get; set; }
    public bool IsArchived { get; set; }

    public ICollection<ConversationMember> Members { get; set; } = new List<ConversationMember>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}

public class ConversationMember
{
    public long ConversationId { get; set; }
    public long UserId { get; set; }
    public string Role { get; set; } = "member";
    public DateTime JoinedAt { get; set; }

    public Conversation? Conversation { get; set; }
    public User? User { get; set; }
}

// Сообщение в диалоге
public class Message
{
    public long Id { get; set; }
    public long ConversationId { get; set; }
    public long SenderId { get; set; }
    public string Body { get; set; } = string.Empty;
    public long? ReplyToId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool Delivered { get; set; }
    public DateTime? ReadAt { get; set; }

    public Conversation? Conversation { get; set; }
    public User? Sender { get; set; }
}

// Уведомление (in-app / push / email / telegram)
public class Notification
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string PayloadJson { get; set; } = string.Empty;
    public string Channel { get; set; } = "inapp";
    public bool Read { get; set; }
    public bool Sent { get; set; }        // через Outbox
    public DateTime CreatedAt { get; set; }

    public User? User { get; set; }
}