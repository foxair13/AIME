using AI.SocialNetwork.Domain.Entities;
using AI.SocialNetwork.Domain.ValueObjects;

namespace AI.SocialNetwork.Application.Contracts;

public interface IUserService
{
    Task<User?> GetByIdAsync(long id);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> CreateAsync(string email, string name, string? city, string? country);
    Task<User?> UpdateAsync(long id, string name, string? bio, string? city, string? country);
    Task<bool> DeleteAsync(long id);
    Task<User?> SetRoleAsync(long id, string role);
    Task<User?> SetActiveAsync(long id, bool isActive);
    Task CommitAsync();
}

public interface IAgentService
{
    Task<Agent?> GetByIdAsync(long id);
    Task<IEnumerable<Agent>> GetByUserIdAsync(long userId);
    Task<Agent?> CreateAsync(long ownerId, string name, string? systemPrompt, string? communicationStyle);
    Task<AgentMessage> SendMessageAsync(long agentId, long? targetAgentId, string content);
    Task<Agent?> UpdateAsync(long id, string? name, string? systemPrompt, string? communicationStyle, bool? isActive, string? llmModel);
    Task<Agent? >ToggleActiveAsync(long agentId, bool isActive);
    Task<IEnumerable<AgentPermission>> GetPermissionsByOwnerAsync(long ownerId);
    Task<AgentPermission?> SetPermissionAsync(long ownerId, string action, bool allowed, decimal? dailyLimitAmount, int? dailyLimitCount);
    Task<IEnumerable<AgentActionLog>> GetActionLogsAsync(long agentId, int limit = 50);
    // A13: проверка лимитов и журналирование решения (explainability)
    Task<PermissionResult> CanExecuteAsync(long agentId, string action, decimal amount, string? reason, string? inputJson);
    Task<AgentActionLog?> LogActionAsync(long agentId, string action, bool allowed, string? inputJson, string? explanation);
}

public record PermissionResult(bool Allowed, string? Reason = null);

// A5/A7/A11: реферальные ссылки, инвойсы, webhook-подписки
public interface IWebhookService
{
    Task<IEnumerable<WebhookSubscription>> GetByUserIdAsync(long userId);
    Task<WebhookSubscription?> SubscribeAsync(long userId, string url, string eventTypes, string? secret);
    Task<bool> UnsubscribeAsync(long subscriptionId, long userId);
    Task<int> DispatchEventAsync(string eventType, string? payloadJson, long? senderUserId = null);
}

public interface IReferralService
{
    Task<ReferralLink?> GetByUserIdAsync(long userId);
    Task<ReferralLink> GetOrCreateAsync(long userId);
    // Активация реферального кода при регистрации нового пользователя
    Task<User?> ApplyCodeAsync(string code, long newUserId);
    Task<IEnumerable<User>> GetReferralsAsync(long userId);
}

public interface IInvoiceService
{
    Task<Invoice?> CreateAsync(long? dealId, string number, decimal total, decimal platformFee, decimal taxRate);
    Task<Invoice?> MarkPaidAsync(long invoiceId);
    Task<IEnumerable<Invoice>> GetAllAsync();
}

public interface ISkillService
{
    Task<Skill?> GetSkillAsync(long id);
    Task<IEnumerable<SkillCategory>> GetCategoriesAsync();
    Task<IEnumerable<Skill>> GetByCategoryAsync(long categoryId);
    Task<Skill?> AddSkillAsync(string name, long? parentId, long categoryId, string? description);
    Task<UserSkill?> AddUserSkillAsync(long userId, long skillId, int level);
    Task<IEnumerable<UserSkill>> GetUserSkillsAsync(long userId);
}

public interface ICompetenceService
{
    Task<decimal> GetRoleFitAsync(long userId, long roleId);      // формула (3.1)
    Task<decimal> GetCurrentLevelAsync(long userId, long skillId); // кривая обучения/забывания (3.27–3.30)
    Task<IEnumerable<ProfessionalRole>> GetRolesAsync();
}

public interface IConnectionService
{
    Task<IEnumerable<Connection>> GetByUserIdAsync(long userId);
    Task<Connection?> CreateAsync(long userFromId, long userToId, string type);
    Task<decimal> GetTrustScoreAsync(long userFromId, long userToId);
}

public interface ISocialService
{
    Task<Post?> CreatePostAsync(long authorId, long? groupId, string body, string contentType, bool isPublic);
    Task<IEnumerable<Post>> GetFeedAsync(long userId, int skip, int take);
    Task<Group?> CreateGroupAsync(long ownerId, string name, bool isPublic);
    Task<Membership?> JoinGroupAsync(long groupId, long userId);
    Task<Forum?> CreateForumAsync(long groupId, string name);
    Task<ForumTopic?> CreateTopicAsync(long categoryId, long authorId, string title);
    Task<Subscription?> SubscribeAsync(long userId, long targetUserId, string targetType);
}

public interface IDealService
{
    Task<Deal?> CreateAsync(long clientId, long contractorId, decimal amount, string? contractTerms);
    Task<Deal?> EscrowAsync(long dealId, decimal amount);
    Task<Deal?> AddMilestoneAsync(long dealId, string title, decimal amount);
    Task<Deal?> CompleteMilestoneAsync(long milestoneId);
    Task<Deal?> CompleteAsync(long dealId);
    Task<Deal?> DisputeAsync(long dealId);
}

public interface INotificationService
{
    Task<Notification> CreateAsync(long userId, string type, string payloadJson, string channel);
    Task<IEnumerable<Notification>> GetUnreadAsync(long userId);
    Task<IEnumerable<Notification>> GetAllAsync(long userId);
    Task MarkReadAsync(long notificationId);
}

public interface IMessageService
{
    Task<IEnumerable<Message>> GetConversationAsync(long conversationId);
    Task<Message> SendAsync(long conversationId, long senderId, string body);
    Task<Conversation?> CreateConversationAsync(long userIdA, long userIdB);
}

public interface IReputationService
{
    Task<IEnumerable<ReputationEntry>> GetHistoryAsync(long userId, long skillId);
    Task<ReputationEntry> AddEntryAsync(long userId, long skillId, decimal value, string source);
    Task<ReputationScore> GetScoreAsync(long userId, long skillId);
}

public interface IMathService
{
    decimal RoleFit(decimal[] weights, decimal[] scores, decimal[] maxScores);         // (3.1)
    decimal Normalize(decimal x, decimal min, decimal max);                            // (3.2) больше — лучше
    decimal NormalizeDescending(decimal x, decimal min, decimal max);                  // (3.3) меньше — лучше
    decimal LearningCurve(decimal maxLevel, decimal startLevel, decimal learningRate, decimal days);       // (3.28)
    decimal DaysToReach(decimal maxLevel, decimal startLevel, decimal targetLevel, decimal learningRate);  // (3.29)
    decimal DecayedLevel(decimal currentLevel, DateTime lastConfirmedAt, decimal decayRate, DateTime now, decimal floorLevel = 0m); // (3.30)

    [Obsolete("Семантически неверно. Используйте DecayedLevel (3.30) или LearningCurve (3.28).")]
    decimal CurrentLevel(decimal maxLevel, decimal currentLevel, DateTime lastConfirmedAt, decimal decayRate, DateTime now);
    double HaversineDistance(double lat1, double lon1, double lat2, double lon2);
    // C4: кластеризация пользователей по профилям (география или навыки)
    int[] KMeansCluster(double[][] points, int k, int maxIterations = 100);
    // C4: Branch&Bound — оптимальный подбор сделки/набор элемент (задача о рюкзаке 0/1)
    KnapsackResult BranchAndBoundKnapsack(double[] weights, double[] values, double capacity);
}

// Результат решения задачи о рюкзаке методом ветвей и границ
public record KnapsackResult(double TotalValue, double TotalWeight, int[] SelectedIndices);

/// <summary>
/// Параметры кандидата как звена контура управления (диссертация, гл. 2, рис. 3, с. 66).
/// </summary>
/// <param name="Competence">Kкомпет — коэффициент интегральной составляющей [0.01..1].</param>
/// <param name="Motivation">Tмотив — коэффициент пропорциональной составляющей [0.1..20].</param>
/// <param name="EnvironmentInertia">Tэ — инерция среды/экономики (мес.), по умолчанию 0.5.</param>
/// <param name="ObjectInertia">Тоб — инерция объекта управления (мес.), по умолчанию 15.</param>
/// <param name="ObjectGain">Kоб — коэффициент передачи объекта, по умолчанию 1.</param>
/// <param name="RequiredLevel">r — требуемый уровень эффективности, по умолчанию 1.</param>
/// <param name="HorizonMonths">Горизонт планирования в месяцах.</param>
public record DynamicFitInput(
    double Competence,
    double Motivation,
    double EnvironmentInertia = 0.5,
    double ObjectInertia = 15.0,
    double ObjectGain = 1.0,
    double RequiredLevel = 1.0,
    double HorizonMonths = 36.0);

/// <param name="LossArea">Интегральные потери ∫max(r−y,0)dt — главный критерий отбора.</param>
/// <param name="TimeToCompetenceMonths">Время вхождения в должность (выход на 95 % от r).</param>
/// <param name="SteadyStateLevel">Установившийся уровень эффективности.</param>
/// <param name="OvershootPercent">Перерегулирование, % — риск «выгорания»/избыточной активности.</param>
/// <param name="Trajectory">Прореженная траектория y(t) для графика.</param>
public record DynamicFitResult(
    double LossArea,
    double TimeToCompetenceMonths,
    double SteadyStateLevel,
    double OvershootPercent,
    IReadOnlyList<double> Trajectory);

public record CandidateProfile(long CandidateId, string DisplayName, DynamicFitInput Input);

public record RankedCandidate(long CandidateId, string DisplayName, DynamicFitResult Result, int Rank);

/// <summary>
/// Динамическая оценка пригодности кадра (DFS) — отбор по площади потерь, а не по баллам.
/// </summary>
public interface IDynamicFitService
{
    DynamicFitResult Evaluate(DynamicFitInput input);
    decimal RotationCost(DynamicFitInput input, decimal vacancyMonths, decimal actingEfficiency, decimal monthlyValue);
    IReadOnlyList<RankedCandidate> Rank(IEnumerable<CandidateProfile> candidates, double horizonMonths);
}

public interface IAuthService
{
    Task<AuthResult?> RegisterAsync(string email, string name, string password);
    Task<AuthResult?> LoginAsync(string email, string password);
    Task<User?> GetByIdAsync(long id);
}

public record SearchHit(long Id, string Type, string Title, string? Summary, decimal Score);

// C5: генерация Word-документов (сертификаты, акты)
public interface ICertificateService
{
    // Возвращает байты .docx с сертификатом об аттестации навыка
    byte[] GenerateAttestationCertificate(string userName, string skillName, DateTime issuedAt, decimal score, string certificateNumber);
    // Возвращает байты .docx с актом о выполнении сделки
    byte[] GenerateDealCertificate(string buyerName, string sellerName, string dealTitle, decimal amount, DateTime completedAt);
}

public interface ISearchService
{
    Task<IEnumerable<SearchHit>> SearchAsync(string query, string type, int skip, int take);
    Task<Hashtag?> GetOrAddHashtagAsync(string tag);
    Task<IEnumerable<SearchHit>> TrendingHashtagsAsync(int take);
}

// A3: LLM-модерация контента (вердикт классификации)
public sealed record ModerationResult(string Status, string? Reason)
{
    public static ModerationResult Pending() => new("pending", null);
    public static ModerationResult Skipped(string reason) => new("skipped", reason);
    public bool IsAllowed => Status == "approved";
}

public interface IModerationService
{
    Task<ModerationResult> ModerateTextAsync(string content, CancellationToken ct = default);
}