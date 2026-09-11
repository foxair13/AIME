using AI.SocialNetwork.Domain.ValueObjects;

namespace AI.SocialNetwork.Domain.Entities;

// Digital Twin — корень агрегата
public class User
{
    public long Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = UserRoles.User;
    public string? Bio { get; set; }
    public Coordinates? Location { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public int TrustLevel { get; set; }          // TL0..TL4 (аудит: безопасность)

    public ICollection<Agent> Agents { get; set; } = new List<Agent>();
    public ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();
    public ICollection<ReputationEntry> ReputationEntries { get; set; } = new List<ReputationEntry>();
    public ICollection<Portfolio> Portfolios { get; set; } = new List<Portfolio>();
    public ICollection<Connection> ConnectionsFrom { get; set; } = new List<Connection>();
    public ICollection<Connection> ConnectionsTo { get; set; } = new List<Connection>();
    public ICollection<AgentPermission> AgentPermissions { get; set; } = new List<AgentPermission>();
    public ICollection<ConsentRecord> Consents { get; set; } = new List<ConsentRecord>();
    public ICollection<UserLanguage> Languages { get; set; } = new List<UserLanguage>();
}