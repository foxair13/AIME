namespace AI.SocialNetwork.Domain.Entities;

// Портфолио
public class Portfolio
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Url { get; set; }
    public DateTime CreatedAt { get; set; }

    public User? User { get; set; }
}