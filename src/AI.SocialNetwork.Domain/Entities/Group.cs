namespace AI.SocialNetwork.Domain.Entities;

// Группа/сообщество (аналог Space HumHub)
public class Group
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public long OwnerId { get; set; }

    public User? Owner { get; set; }
    public ICollection<GroupPermission> Permissions { get; set; } = new List<GroupPermission>();
    public ICollection<Membership> Members { get; set; } = new List<Membership>();
    public ICollection<Post> Posts { get; set; } = new List<Post>();
}

// Членство в группе
public class Membership
{
    public long GroupId { get; set; }
    public long UserId { get; set; }
    public string Role { get; set; } = "member";   // Owner/Admin/Mod/Member/Viewer
    public DateTime JoinedAt { get; set; }

    public Group? Group { get; set; }
    public User? User { get; set; }
}