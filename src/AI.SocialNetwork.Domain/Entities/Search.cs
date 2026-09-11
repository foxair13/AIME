namespace AI.SocialNetwork.Domain.Entities;

// Хэштег для постов/групп
public class Hashtag
{
    public long Id { get; set; }
    public string Tag { get; set; } = string.Empty;
    public int UsageCount { get; set; }         // тренды
    public DateTime CreatedAt { get; set; }

    public ICollection<PostHashtag> Posts { get; set; } = new List<PostHashtag>();
}

// Связь пост ↔ хэштег
public class PostHashtag
{
    public long PostId { get; set; }
    public long HashtagId { get; set; }
    public DateTime CreatedAt { get; set; }

    public Post? Post { get; set; }
    public Hashtag? Hashtag { get; set; }
}