using AI.SocialNetwork.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AI.SocialNetwork.Infrastructure.DataContext;

public partial class AISocialNetworkContext : DbContext
{
    public AISocialNetworkContext(DbContextOptions<AISocialNetworkContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Agent> Agents { get; set; }
    public DbSet<AgentMessage> AgentMessages { get; set; }
    public DbSet<Connection> Connections { get; set; }
    public DbSet<Portfolio> Portfolios { get; set; }
    public DbSet<ReputationEntry> ReputationEntries { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<SkillClosure> SkillClosures { get; set; }
    public DbSet<SkillCategory> SkillCategories { get; set; }
    public DbSet<UserSkill> UserSkills { get; set; }
    public DbSet<ProfessionalRole> ProfessionalRoles { get; set; }
    public DbSet<RoleSkillWeight> RoleSkillWeights { get; set; }
    public DbSet<Attestation> Attestations { get; set; }
    public DbSet<DevelopmentPlan> DevelopmentPlans { get; set; }
    public DbSet<PlanStep> PlanSteps { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Membership> Memberships { get; set; }
    public DbSet<GroupPermission> GroupPermissions { get; set; }
    public DbSet<Forum> Forums { get; set; }
    public DbSet<ForumCategory> ForumCategories { get; set; }
    public DbSet<ForumPermission> ForumPermissions { get; set; }
    public DbSet<ForumTopic> ForumTopics { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Meeting> Meetings { get; set; }
    public DbSet<Deal> Deals { get; set; }
    public DbSet<DealMilestone> DealMilestones { get; set; }
    public DbSet<Board> Boards { get; set; }
    public DbSet<BoardColumn> BoardColumns { get; set; }
    public DbSet<BoardCard> BoardCards { get; set; }
    public DbSet<UserFile> UserFiles { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<ConversationMember> ConversationMembers { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<KYCRequest> KYCRequests { get; set; }
    public DbSet<ConsentRecord> ConsentRecords { get; set; }
    public DbSet<AuditLogEntry> AuditLogEntries { get; set; }
    public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
    public DbSet<PlanSubscriber> PlanSubscribers { get; set; }
    public DbSet<Donation> Donations { get; set; }
    public DbSet<ReferralLink> ReferralLinks { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<RefundRequest> RefundRequests { get; set; }
    public DbSet<WebhookSubscription> WebhookSubscriptions { get; set; }
    public DbSet<AgentPermission> AgentPermissions { get; set; }
    public DbSet<AgentActionLog> AgentActionLogs { get; set; }
    public DbSet<AnalyticsEvent> AnalyticsEvents { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<UserLanguage> UserLanguages { get; set; }
    public DbSet<Translation> Translations { get; set; }
    public DbSet<ContentLanguage> ContentLanguages { get; set; }
    public DbSet<Hashtag> Hashtags { get; set; }
    public DbSet<PostHashtag> PostHashtags { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AISocialNetworkContext).Assembly);
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}