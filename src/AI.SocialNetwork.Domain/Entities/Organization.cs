namespace AI.SocialNetwork.Domain.Entities;

// Организация
public class Organization
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public long OwnerId { get; set; }

    public User? Owner { get; set; }
    public ICollection<User> Members { get; set; } = new List<User>();
}