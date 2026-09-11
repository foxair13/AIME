using AI.SocialNetwork.Domain.Entities;
using AI.SocialNetwork.Infrastructure.Repositories.Contracts;

namespace AI.SocialNetwork.Infrastructure.UnitOfWork.Contracts;

public interface IUnitOfWork
{
    IGenericRepository<User> Users { get; }
    IGenericRepository<Agent> Agents { get; }
    IGenericRepository<AgentMessage> AgentMessages { get; }
    IGenericRepository<Connection> Connections { get; }
    IGenericRepository<Portfolio> Portfolios { get; }
    IGenericRepository<ReputationEntry> ReputationEntries { get; }
    IGenericRepository<Skill> Skills { get; }
    IGenericRepository<SkillClosure> SkillClosures { get; }
    IGenericRepository<SkillCategory> SkillCategories { get; }
    IGenericRepository<UserSkill> UserSkills { get; }
    IGenericRepository<ProfessionalRole> ProfessionalRoles { get; }
    IGenericRepository<RoleSkillWeight> RoleSkillWeights { get; }
    IGenericRepository<Attestation> Attestations { get; }
    IGenericRepository<DevelopmentPlan> DevelopmentPlans { get; }
    IGenericRepository<PlanStep> PlanSteps { get; }
    IGenericRepository<Organization> Organizations { get; }
    IGenericRepository<Post> Posts { get; }
    IGenericRepository<Group> Groups { get; }
    IGenericRepository<Membership> Memberships { get; }
    IGenericRepository<GroupPermission> GroupPermissions { get; }
    IGenericRepository<Forum> Forums { get; }
    IGenericRepository<ForumCategory> ForumCategories { get; }
    IGenericRepository<ForumPermission> ForumPermissions { get; }
    IGenericRepository<ForumTopic> ForumTopics { get; }
    IGenericRepository<Subscription> Subscriptions { get; }
    IGenericRepository<Meeting> Meetings { get; }
    IGenericRepository<Deal> Deals { get; }
    IGenericRepository<DealMilestone> DealMilestones { get; }
    IGenericRepository<Board> Boards { get; }
    IGenericRepository<BoardColumn> BoardColumns { get; }
    IGenericRepository<BoardCard> BoardCards { get; }
    IGenericRepository<UserFile> UserFiles { get; }
    IGenericRepository<Conversation> Conversations { get; }
    IGenericRepository<ConversationMember> ConversationMembers { get; }
    IGenericRepository<Message> Messages { get; }
    IGenericRepository<Notification> Notifications { get; }
    IGenericRepository<Report> Reports { get; }
    IGenericRepository<Review> Reviews { get; }
    IGenericRepository<KYCRequest> KYCRequests { get; }
    IGenericRepository<ConsentRecord> ConsentRecords { get; }
    IGenericRepository<AuditLogEntry> AuditLogEntries { get; }
    IGenericRepository<SubscriptionPlan> SubscriptionPlans { get; }
    IGenericRepository<PlanSubscriber> PlanSubscribers { get; }
    IGenericRepository<Donation> Donations { get; }
    IGenericRepository<ReferralLink> ReferralLinks { get; }
    IGenericRepository<Invoice> Invoices { get; }
    IGenericRepository<RefundRequest> RefundRequests { get; }
    IGenericRepository<WebhookSubscription> WebhookSubscriptions { get; }
    IGenericRepository<AgentPermission> AgentPermissions { get; }
    IGenericRepository<AgentActionLog> AgentActionLogs { get; }
    IGenericRepository<AnalyticsEvent> AnalyticsEvents { get; }
    IGenericRepository<Language> Languages { get; }
    IGenericRepository<UserLanguage> UserLanguages { get; }
    IGenericRepository<Translation> Translations { get; }
    IGenericRepository<ContentLanguage> ContentLanguages { get; }

    IGenericRepository<Hashtag> Hashtags { get; }
    IGenericRepository<PostHashtag> PostHashtags { get; }

    IGenericRepository<OutboxMessage> OutboxMessages { get; }

    Task CommitAsync();
}