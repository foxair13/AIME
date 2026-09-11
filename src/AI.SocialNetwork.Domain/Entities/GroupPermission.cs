namespace AI.SocialNetwork.Domain.Entities;

// Право группы на действие/событие
public class GroupPermission
{
    public long GroupId { get; set; }
    public string RoleId { get; set; } = string.Empty;   // "owner", "member"...
    public string Action { get; set; } = string.Empty;   // "post", "comment", "moderate", "invite"...
    public bool Allowed { get; set; }

    public Group? Group { get; set; }
}