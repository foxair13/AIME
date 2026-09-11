namespace AI.SocialNetwork.Domain.Entities;

// Пост (контент)
public class Post
{
    public long Id { get; set; }
    public long AuthorId { get; set; }
    public long? GroupId { get; set; }          // null = личная страница/лента
    public string Body { get; set; } = string.Empty;
    public string ContentType { get; set; } = "text";   // text/dtphoto/video/pdf/poll/event
    public long? FileId { get; set; }           // PDF-резюме и пр.
    public string? LanguageCode { get; set; }   // язык контента (i18n)
    public bool IsPublic { get; set; }
    public bool IsModerated { get; set; }       // прошла LLM-фильтр модерации
    public string ModerationStatus { get; set; } = "pending";   // pending/approved/flagged/rejected/skipped
    public string? ModerationReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PinnedAt { get; set; }

    public User? Author { get; set; }
    public Group? Group { get; set; }
    public ICollection<ContentLanguage> ContentLanguages { get; set; } = new List<ContentLanguage>();
    public ICollection<PostHashtag> Hashtags { get; set; } = new List<PostHashtag>();
}