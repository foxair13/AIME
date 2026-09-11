using AI.SocialNetwork.Domain.Entities;
using AI.SocialNetwork.Infrastructure.DataContext;
using AI.SocialNetwork.Infrastructure.Repositories.Contracts;
using AI.SocialNetwork.Infrastructure.UnitOfWork.Contracts;

namespace AI.SocialNetwork.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly AISocialNetworkContext _context;

    public UnitOfWork(
        AISocialNetworkContext context,
        IGenericRepository<User> usersRepository,
        IGenericRepository<Agent> agentsRepository,
        IGenericRepository<AgentMessage> agentMessagesRepository,
        IGenericRepository<Connection> connectionsRepository,
        IGenericRepository<Portfolio> portfoliosRepository,
        IGenericRepository<ReputationEntry> reputationEntriesRepository,
        IGenericRepository<Skill> skillsRepository,
        IGenericRepository<SkillClosure> skillClosuresRepository,
        IGenericRepository<SkillCategory> skillCategoriesRepository,
        IGenericRepository<UserSkill> userSkillsRepository,
        IGenericRepository<ProfessionalRole> professionalRolesRepository,
        IGenericRepository<RoleSkillWeight> roleSkillWeightsRepository,
        IGenericRepository<Attestation> attestationsRepository,
        IGenericRepository<DevelopmentPlan> developmentPlansRepository,
        IGenericRepository<PlanStep> planStepsRepository,
        IGenericRepository<Organization> organizationsRepository,
        IGenericRepository<Post> postsRepository,
        IGenericRepository<Group> groupsRepository,
        IGenericRepository<Membership> membershipsRepository,
        IGenericRepository<GroupPermission> groupPermissionsRepository,
        IGenericRepository<Forum> forumsRepository,
        IGenericRepository<ForumCategory> forumCategoriesRepository,
        IGenericRepository<ForumPermission> forumPermissionsRepository,
        IGenericRepository<ForumTopic> forumTopicsRepository,
        IGenericRepository<Subscription> subscriptionsRepository,
        IGenericRepository<Meeting> meetingsRepository,
        IGenericRepository<Deal> dealsRepository,
        IGenericRepository<DealMilestone> dealMilestonesRepository,
        IGenericRepository<Board> boardsRepository,
        IGenericRepository<BoardColumn> boardColumnsRepository,
        IGenericRepository<BoardCard> boardCardsRepository,
        IGenericRepository<UserFile> userFilesRepository,
        IGenericRepository<Conversation> conversationsRepository,
        IGenericRepository<ConversationMember> conversationMembersRepository,
        IGenericRepository<Message> messagesRepository,
        IGenericRepository<Notification> notificationsRepository,
        IGenericRepository<Report> reportsRepository,
        IGenericRepository<Review> reviewsRepository,
        IGenericRepository<KYCRequest> kycRequestsRepository,
        IGenericRepository<ConsentRecord> consentRecordsRepository,
        IGenericRepository<AuditLogEntry> auditLogEntriesRepository,
        IGenericRepository<SubscriptionPlan> subscriptionPlansRepository,
        IGenericRepository<PlanSubscriber> planSubscribersRepository,
        IGenericRepository<Donation> donationsRepository,
        IGenericRepository<ReferralLink> referralLinksRepository,
        IGenericRepository<Invoice> invoicesRepository,
        IGenericRepository<RefundRequest> refundRequestsRepository,
        IGenericRepository<WebhookSubscription> webhookSubscriptionsRepository,
        IGenericRepository<AgentPermission> agentPermissionsRepository,
        IGenericRepository<AgentActionLog> agentActionLogsRepository,
        IGenericRepository<AnalyticsEvent> analyticsEventsRepository,
        IGenericRepository<Language> languagesRepository,
        IGenericRepository<UserLanguage> userLanguagesRepository,
        IGenericRepository<Translation> translationsRepository,
        IGenericRepository<ContentLanguage> contentLanguagesRepository,
        IGenericRepository<Hashtag> hashtagsRepository,
        IGenericRepository<PostHashtag> postHashtagsRepository,
        IGenericRepository<OutboxMessage> outboxMessagesRepository
    )
    {
        _context = context;
        Users = usersRepository;
        Agents = agentsRepository;
        AgentMessages = agentMessagesRepository;
        Connections = connectionsRepository;
        Portfolios = portfoliosRepository;
        ReputationEntries = reputationEntriesRepository;
        Skills = skillsRepository;
        SkillClosures = skillClosuresRepository;
        SkillCategories = skillCategoriesRepository;
        UserSkills = userSkillsRepository;
        ProfessionalRoles = professionalRolesRepository;
        RoleSkillWeights = roleSkillWeightsRepository;
        Attestations = attestationsRepository;
        DevelopmentPlans = developmentPlansRepository;
        PlanSteps = planStepsRepository;
        Organizations = organizationsRepository;
        Posts = postsRepository;
        Groups = groupsRepository;
        Memberships = membershipsRepository;
        GroupPermissions = groupPermissionsRepository;
        Forums = forumsRepository;
        ForumCategories = forumCategoriesRepository;
        ForumPermissions = forumPermissionsRepository;
        ForumTopics = forumTopicsRepository;
        Subscriptions = subscriptionsRepository;
        Meetings = meetingsRepository;
        Deals = dealsRepository;
        DealMilestones = dealMilestonesRepository;
        Boards = boardsRepository;
        BoardColumns = boardColumnsRepository;
        BoardCards = boardCardsRepository;
        UserFiles = userFilesRepository;
        Conversations = conversationsRepository;
        ConversationMembers = conversationMembersRepository;
        Messages = messagesRepository;
        Notifications = notificationsRepository;
        Reports = reportsRepository;
        Reviews = reviewsRepository;
        KYCRequests = kycRequestsRepository;
        ConsentRecords = consentRecordsRepository;
        AuditLogEntries = auditLogEntriesRepository;
        SubscriptionPlans = subscriptionPlansRepository;
        PlanSubscribers = planSubscribersRepository;
        Donations = donationsRepository;
        ReferralLinks = referralLinksRepository;
        Invoices = invoicesRepository;
        RefundRequests = refundRequestsRepository;
        WebhookSubscriptions = webhookSubscriptionsRepository;
        AgentPermissions = agentPermissionsRepository;
        AgentActionLogs = agentActionLogsRepository;
        AnalyticsEvents = analyticsEventsRepository;
        Languages = languagesRepository;
        UserLanguages = userLanguagesRepository;
        Translations = translationsRepository;
        ContentLanguages = contentLanguagesRepository;
        Hashtags = hashtagsRepository;
        PostHashtags = postHashtagsRepository;
        OutboxMessages = outboxMessagesRepository;
    }

    public IGenericRepository<User> Users { get; }
    public IGenericRepository<Agent> Agents { get; }
    public IGenericRepository<AgentMessage> AgentMessages { get; }
    public IGenericRepository<Connection> Connections { get; }
    public IGenericRepository<Portfolio> Portfolios { get; }
    public IGenericRepository<ReputationEntry> ReputationEntries { get; }
    public IGenericRepository<Skill> Skills { get; }
    public IGenericRepository<SkillClosure> SkillClosures { get; }
    public IGenericRepository<SkillCategory> SkillCategories { get; }
    public IGenericRepository<UserSkill> UserSkills { get; }
    public IGenericRepository<ProfessionalRole> ProfessionalRoles { get; }
    public IGenericRepository<RoleSkillWeight> RoleSkillWeights { get; }
    public IGenericRepository<Attestation> Attestations { get; }
    public IGenericRepository<DevelopmentPlan> DevelopmentPlans { get; }
    public IGenericRepository<PlanStep> PlanSteps { get; }
    public IGenericRepository<Organization> Organizations { get; }
    public IGenericRepository<Post> Posts { get; }
    public IGenericRepository<Group> Groups { get; }
    public IGenericRepository<Membership> Memberships { get; }
    public IGenericRepository<GroupPermission> GroupPermissions { get; }
    public IGenericRepository<Forum> Forums { get; }
    public IGenericRepository<ForumCategory> ForumCategories { get; }
    public IGenericRepository<ForumPermission> ForumPermissions { get; }
    public IGenericRepository<ForumTopic> ForumTopics { get; }
    public IGenericRepository<Subscription> Subscriptions { get; }
    public IGenericRepository<Meeting> Meetings { get; }
    public IGenericRepository<Deal> Deals { get; }
    public IGenericRepository<DealMilestone> DealMilestones { get; }
    public IGenericRepository<Board> Boards { get; }
    public IGenericRepository<BoardColumn> BoardColumns { get; }
    public IGenericRepository<BoardCard> BoardCards { get; }
    public IGenericRepository<UserFile> UserFiles { get; }
    public IGenericRepository<Conversation> Conversations { get; }
    public IGenericRepository<ConversationMember> ConversationMembers { get; }
    public IGenericRepository<Message> Messages { get; }
    public IGenericRepository<Notification> Notifications { get; }
    public IGenericRepository<Report> Reports { get; }
    public IGenericRepository<Review> Reviews { get; }
    public IGenericRepository<KYCRequest> KYCRequests { get; }
    public IGenericRepository<ConsentRecord> ConsentRecords { get; }
    public IGenericRepository<AuditLogEntry> AuditLogEntries { get; }
    public IGenericRepository<SubscriptionPlan> SubscriptionPlans { get; }
    public IGenericRepository<PlanSubscriber> PlanSubscribers { get; }
    public IGenericRepository<Donation> Donations { get; }
    public IGenericRepository<ReferralLink> ReferralLinks { get; }
    public IGenericRepository<Invoice> Invoices { get; }
    public IGenericRepository<RefundRequest> RefundRequests { get; }
    public IGenericRepository<WebhookSubscription> WebhookSubscriptions { get; }
    public IGenericRepository<AgentPermission> AgentPermissions { get; }
    public IGenericRepository<AgentActionLog> AgentActionLogs { get; }
    public IGenericRepository<AnalyticsEvent> AnalyticsEvents { get; }
    public IGenericRepository<Language> Languages { get; }
    public IGenericRepository<UserLanguage> UserLanguages { get; }
    public IGenericRepository<Translation> Translations { get; }
    public IGenericRepository<ContentLanguage> ContentLanguages { get; }
    public IGenericRepository<Hashtag> Hashtags { get; }
    public IGenericRepository<PostHashtag> PostHashtags { get; }
    public IGenericRepository<OutboxMessage> OutboxMessages { get; }

    public async Task CommitAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}