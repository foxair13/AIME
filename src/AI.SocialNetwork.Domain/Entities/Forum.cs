namespace AI.SocialNetwork.Domain.Entities;

// Форум (категории/темы/посты — как Discourse)
public class Forum
{
    public long Id { get; set; }
    public long GroupId { get; set; }
    public string Name { get; set; } = string.Empty;

    public Group? Group { get; set; }
    public ICollection<ForumCategory> Categories { get; set; } = new List<ForumCategory>();
}

public class ForumCategory
{
    public long Id { get; set; }
    public long ForumId { get; set; }
    public string Name { get; set; } = string.Empty;

    public Forum? Forum { get; set; }
    public ICollection<ForumPermission> Permissions { get; set; } = new List<ForumPermission>();
    public ICollection<ForumTopic> Topics { get; set; } = new List<ForumTopic>();
}

public class ForumPermission
{
    public long CategoryId { get; set; }
    public string RoleId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public bool Allowed { get; set; }

    public ForumCategory? Category { get; set; }
}

public class ForumTopic
{
    public long Id { get; set; }
    public long CategoryId { get; set; }
    public string Title { get; set; } = string.Empty;
    public long AuthorId { get; set; }
    public DateTime CreatedAt { get; set; }

    public ForumCategory? Category { get; set; }
    public User? Author { get; set; }
}