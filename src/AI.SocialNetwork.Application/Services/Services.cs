using AI.SocialNetwork.Application.Contracts;
using AI.SocialNetwork.Domain.Entities;
using AI.SocialNetwork.Domain.ValueObjects;
using AI.SocialNetwork.Infrastructure.Repositories.Contracts;
using AI.SocialNetwork.Infrastructure.UnitOfWork.Contracts;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AI.SocialNetwork.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _uow;

    public UserService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<User?> GetByIdAsync(long id)
    {
        return await _uow.Users.GetAsync(id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var users = await _uow.Users.GetAllAsync();
        return users.FirstOrDefault(x => x.Email == email);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _uow.Users.GetAllAsync();
    }

    public async Task<User?> CreateAsync(string email, string name, string? city, string? country)
    {
        var user = new User
        {
            Email = email,
            Name = name,
            City = city,
            Country = country,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        await _uow.Users.Add(user);
        await _uow.CommitAsync();

        // Событие в брокер (user.registered -> аналитика регистраций)
        await _uow.OutboxMessages.Add(new OutboxMessage
        {
            EventType = "user.registered",
            AggregateId = user.Id.ToString(),
            PayloadJson = System.Text.Json.JsonSerializer.Serialize(new { UserId = user.Id, user.Email }),
            Destination = "kafka:analytics-events",
            CreatedAt = DateTime.UtcNow
        });
        await _uow.CommitAsync();
        return user;
    }

    public async Task<User?> UpdateAsync(long id, string name, string? bio, string? city, string? country)
    {
        var user = await _uow.Users.GetAsync(id);
        if (user == null)
        {
            return null;
        }
        user.Name = name;
        user.Bio = bio;
        user.City = city;
        user.Country = country;
        _uow.Users.Update(user);
        await _uow.CommitAsync();
        return user;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var user = await _uow.Users.GetAsync(id);
        if (user == null)
        {
            return false;
        }
        _uow.Users.Delete(user);
        await _uow.CommitAsync();
        return true;
    }

    public async Task<User?> SetRoleAsync(long id, string role)
    {
        var user = await _uow.Users.GetAsync(id);
        if (user == null)
        {
            return null;
        }
        user.Role = role;
        _uow.Users.Update(user);
        await _uow.CommitAsync();
        return user;
    }

    public async Task<User?> SetActiveAsync(long id, bool isActive)
    {
        var user = await _uow.Users.GetAsync(id);
        if (user == null)
        {
            return null;
        }
        user.IsActive = isActive;
        _uow.Users.Update(user);
        await _uow.CommitAsync();
        return user;
    }

    public Task CommitAsync()
    {
        return _uow.CommitAsync();
    }
}

public class AgentService : IAgentService
{
    private readonly IUnitOfWork _uow;
    private readonly OllamaClient? _ollama;

    public AgentService(IUnitOfWork uow, OllamaClient? ollama = null)
    {
        _uow = uow;
        _ollama = ollama;
    }

    public async Task<Agent?> GetByIdAsync(long id)
    {
        return await _uow.Agents.GetAsync(id);
    }

    public async Task<IEnumerable<Agent>> GetByUserIdAsync(long userId)
    {
        return await _uow.Agents.GetAllAsync().ContinueWith(t => t.Result.Where(a => a.UserId == userId));
    }

    public async Task<Agent?> CreateAsync(long ownerId, string name, string? systemPrompt, string? communicationStyle)
    {
        var agent = new Agent
        {
            UserId = ownerId,
            Name = name,
            SystemPrompt = systemPrompt,
            CommunicationStyle = communicationStyle,
            LLMProvider = "ollama",
            LLMModel = "nemotron-mini",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        await _uow.Agents.Add(agent);
        await _uow.CommitAsync();
        return agent;
    }

    public async Task<Agent?> UpdateAsync(long id, string? name, string? systemPrompt, string? communicationStyle, bool? isActive, string? llmModel)
    {
        var agent = await _uow.Agents.GetAsync(id);
        if (agent == null)
        {
            return null;
        }
        if (name != null) agent.Name = name;
        if (systemPrompt != null) agent.SystemPrompt = systemPrompt;
        if (communicationStyle != null) agent.CommunicationStyle = communicationStyle;
        if (isActive.HasValue) agent.IsActive = isActive.Value;
        if (llmModel != null) agent.LLMModel = llmModel;
        _uow.Agents.Update(agent);
        await _uow.CommitAsync();
        return agent;
    }

    public async Task<Agent?> ToggleActiveAsync(long agentId, bool isActive)
    {
        return await UpdateAsync(agentId, null, null, null, isActive, null);
    }

    public async Task<AgentMessage> SendMessageAsync(long agentId, long? targetAgentId, string content)
    {
        // A13: проверяем право send_message и журналируем решение
        var check = await CanExecuteAsync(agentId, "send_message", 0, null, null);
        if (!check.Allowed)
        {
            throw new UnauthorizedAccessException(check.Reason ?? "Действие не разрешено");
        }

        var agent = await _uow.Agents.GetAsync(agentId);
        var systemPrompt = agent?.SystemPrompt ?? $"Ты — {agent?.Name ?? "AI-агент"}. Будь полезным и кратким.";

        string reply;
        string explanation;
        if (_ollama != null)
        {
            var (ok, _, aiReply) = await _ollama.ChatAsync(agent?.LLMModel ?? "nemotron-mini", systemPrompt, content);
            if (ok && !string.IsNullOrWhiteSpace(aiReply))
            {
                reply = aiReply!;
                explanation = "Ответ сгенерирован локальной LLM (Ollama)";
            }
            else
            {
                reply = FallbackReply(agent?.Name, content);
                explanation = "Ollama недоступна — использован локальный fallback-ответ";
            }
        }
        else
        {
            reply = FallbackReply(agent?.Name, content);
            explanation = "LLM не настроена — использован локальный fallback-ответ";
        }

        var message = new AgentMessage
        {
            AgentId = agentId,
            TargetAgentId = targetAgentId,
            Content = reply,
            SentAt = DateTime.UtcNow
        };
        await _uow.AgentMessages.Add(message);
        await _uow.CommitAsync();

        // Событие в брокер (agent.message.sent -> аналитика трафика агентов)
        await _uow.OutboxMessages.Add(new OutboxMessage
        {
            EventType = "agent.message.sent",
            AggregateId = message.Id.ToString(),
            PayloadJson = System.Text.Json.JsonSerializer.Serialize(new { AgentId = agentId, SentAt = DateTime.UtcNow },
                System.Text.Json.JsonSerializerOptions.Default),
            Destination = "kafka:agent-events",
            CreatedAt = DateTime.UtcNow
        });
        await _uow.CommitAsync();
        await LogActionAsync(agentId, "send_message", true, content, explanation);
        return message;
    }

    // Первое «AI»: детерминированный ответ, если локальная LLM не запущена
    private static string FallbackReply(string? agentName, string content)
    {
        var greeting = string.IsNullOrWhiteSpace(agentName) ? "Агент" : agentName;
        return $"{greeting}: вы написали «{(content.Length > 120 ? content[..120] + "…" : content)}». " +
               "Это ответ, сгенерированный локальным движком (демо). Подключите Ollama: docker compose up -d ollama";
    }

    public async Task<IEnumerable<AgentPermission>> GetPermissionsByOwnerAsync(long ownerId)
    {
        return await _uow.AgentPermissions.GetAllAsync().ContinueWith(t => t.Result.Where(p => p.OwnerId == ownerId));
    }

    public async Task<AgentPermission?> SetPermissionAsync(long ownerId, string action, bool allowed, decimal? dailyLimitAmount, int? dailyLimitCount)
    {
        var existing = (await _uow.AgentPermissions.GetAllAsync()).FirstOrDefault(p => p.OwnerId == ownerId && p.Action == action);
        if (existing == null)
        {
            existing = new AgentPermission
            {
                OwnerId = ownerId,
                Action = action,
                Allowed = allowed,
                DailyLimitAmount = dailyLimitAmount,
                DailyLimitCount = dailyLimitCount
            };
            await _uow.AgentPermissions.Add(existing);
        }
        else
        {
            existing.Allowed = allowed;
            existing.DailyLimitAmount = dailyLimitAmount;
            existing.DailyLimitCount = dailyLimitCount;
            _uow.AgentPermissions.Update(existing);
        }
        await _uow.CommitAsync();
        return existing;
    }

    public async Task<IEnumerable<AgentActionLog>> GetActionLogsAsync(long agentId, int limit = 50)
    {
        return (await _uow.AgentActionLogs.GetAllAsync())
            .Where(l => l.AgentId == agentId)
            .OrderByDescending(l => l.CreatedAt)
            .Take(limit);
    }

    // A13: проверка лимита по событию с учётом прав и использованных за сегодня сумм/количества
    public async Task<PermissionResult> CanExecuteAsync(long agentId, string action, decimal amount, string? reason, string? inputJson)
    {
        var agent = await _uow.Agents.GetAsync(agentId);
        if (agent == null)
        {
            return new PermissionResult(false, "Агент не найден");
        }
        if (!agent.IsActive)
        {
            return new PermissionResult(false, "Агент отключён владельцем");
        }

        var allowed = (await _uow.AgentPermissions.GetAllAsync())
            .FirstOrDefault(p => p.OwnerId == agent.UserId && p.Action == action);
        if (allowed == null)
        {
            // Без явного права — по умолчанию безопасная политика (deny-all для финансовых действий)
            var safeByDefault = action is "create_deal" or "pay" or "book_meeting" ? false : true;
            return new PermissionResult(safeByDefault, safeByDefault ? "Нет ограничений" : "Требуется разрешение владельца на действие");
        }
        if (!allowed.Allowed)
        {
            return new PermissionResult(false, $"Действие «{action}» запрещено владельцем");
        }

        // Использование за сегодня (время по UTC)
        var today = DateTime.UtcNow.Date;
        var todayLogs = (await _uow.AgentActionLogs.GetAllAsync())
            .Where(l => l.AgentId == agentId && l.Allowed && l.CreatedAt >= today);

        if (allowed.DailyLimitCount.HasValue)
        {
            var countUsed = todayLogs.Count(l => l.Action == action);
            if (countUsed >= allowed.DailyLimitCount.Value)
            {
                return new PermissionResult(false, $"Превышен дневной лимит операций ({allowed.DailyLimitCount.Value}) для «{action}»");
            }
        }
        if (allowed.DailyLimitAmount.HasValue && amount > 0)
        {
            var amountUsed = todayLogs.Where(l => l.Action == action)
                .Sum(l => decimal.TryParse(l.InputJson, out var v) ? v : 0m) + amount;
            if (amountUsed > allowed.DailyLimitAmount.Value)
            {
                return new PermissionResult(false, $"Превышен дневной лимит суммы для «{action}»");
            }
        }

        return new PermissionResult(true, string.IsNullOrEmpty(reason) ? "Действие разрешено" : reason);
    }

    public async Task<AgentActionLog?> LogActionAsync(long agentId, string action, bool allowed, string? inputJson, string? explanation)
    {
        var agent = await _uow.Agents.GetAsync(agentId);
        if (agent == null)
        {
            return null;
        }
        var log = new AgentActionLog
        {
            AgentId = agentId,
            OwnerId = agent.UserId,
            Action = action,
            InputJson = inputJson,
            Explanation = explanation,
            Allowed = allowed,
            CreatedAt = DateTime.UtcNow
        };
        await _uow.AgentActionLogs.Add(log);
        await _uow.CommitAsync();
        return log;
    }
}

public class CertificateService : ICertificateService
{
    // C5: сертификат об аттестации навыка в формате Word (.docx)
    public byte[] GenerateAttestationCertificate(string userName, string skillName, DateTime issuedAt, decimal score, string certificateNumber)
    {
        using var stream = new MemoryStream();
        using (var document = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document))
        {
            var mainPart = document.AddMainDocumentPart();
            mainPart.Document = new Document(new Body(
                CertificateHeading("СЕРТИФИКАТ ОБ АТТЕСТАЦИИ"),
                CertificateParagraph($"Настоящий сертификат удостоверяет, что\n{userName}\nуспешно прошёл(ла) аттестацию по навыку"),
                CertificateParagraph(skillName, true),
                CertificateParagraph($"Результат: {score:F0} из 100"),
                CertificateParagraph($"Номер сертификата: {certificateNumber}"),
                CertificateParagraph($"Дата выдачи: {issuedAt:dd.MM.yyyy}")
            ));
            mainPart.Document.Save();
        }
        return stream.ToArray();
    }

    // C5: акт о выполнении сделки (для портфеля контрагента)
    public byte[] GenerateDealCertificate(string buyerName, string sellerName, string dealTitle, decimal amount, DateTime completedAt)
    {
        using var stream = new MemoryStream();
        using (var document = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document))
        {
            var mainPart = document.AddMainDocumentPart();
            mainPart.Document = new Document(new Body(
                CertificateHeading("АКТ О ВЫПОЛНЕНИИ СДЕЛКИ"),
                CertificateParagraph($"{sellerName} выполнил(а) для {buyerName} работу по теме:"),
                CertificateParagraph(dealTitle, true),
                CertificateParagraph($"Стоимость: {amount:F2} у.е."),
                CertificateParagraph($"Дата завершения: {completedAt:dd.MM.yyyy}"),
                CertificateParagraph("Обе стороны подтверждают, что обязательства выполнены в полном объёме.")
            ));
            mainPart.Document.Save();
        }
        return stream.ToArray();
    }

    private static Paragraph CertificateHeading(string text)
    {
        return new Paragraph(
            new Run(
                new RunProperties(
                    new Bold(),
                    new FontSize { Val = "36" },
                    new FontSizeComplexScript { Val = "36" }),
                new Text(text) { Space = SpaceProcessingModeValues.Preserve }))
        { ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Center }, new SpacingBetweenLines() { After = "400" }) };
    }

    private static Paragraph CertificateParagraph(string text, bool bold = false)
    {
        var runs = new List<Run>();
        if (bold)
        {
            runs.Add(new Run(new RunProperties(new Bold(), new FontSize { Val = "32" }, new FontSizeComplexScript { Val = "32" }), new Text(text) { Space = SpaceProcessingModeValues.Preserve }));
        }
        else
        {
            runs.Add(new Run(new RunProperties(new FontSize { Val = "28" }, new FontSizeComplexScript { Val = "28" }), new Text(text) { Space = SpaceProcessingModeValues.Preserve }));
        }
        return new Paragraph(runs) { ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Center }, new SpacingBetweenLines() { After = "200" }) };
    }
}

public class ReferralService : IReferralService
{
    private readonly IUnitOfWork _uow;
    private readonly Random _rng = new();

    public ReferralService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<ReferralLink?> GetByUserIdAsync(long userId)
    {
        return (await _uow.ReferralLinks.GetAllAsync()).FirstOrDefault(r => r.UserId == userId);
    }

    public async Task<ReferralLink> GetOrCreateAsync(long userId)
    {
        var existing = (await _uow.ReferralLinks.GetAllAsync()).FirstOrDefault(r => r.UserId == userId);
        if (existing != null)
        {
            return existing;
        }
        string code;
        do
        {
            code = new string(Enumerable.Range(0, 8).Select(_ => (char)_rng.Next(65, 91)).ToArray());
        }
        while ((await _uow.ReferralLinks.GetAllAsync()).Any(r => r.Code == code));

        var link = new ReferralLink { UserId = userId, Code = code };
        await _uow.ReferralLinks.Add(link);
        await _uow.CommitAsync();
        return link;
    }

    public async Task<User?> ApplyCodeAsync(string code, long newUserId)
    {
        var link = (await _uow.ReferralLinks.GetAllAsync()).FirstOrDefault(r => r.Code == code);
        if (link == null || link.UserId == newUserId)
        {
            return null;
        }

        // Связь referrer -> новичок (Connection типа "referral")
        var existing = (await _uow.Connections.GetAllAsync())
            .Any(c => c.UserFromId == link.UserId && c.UserToId == newUserId && c.ConnectionType == "referral");
        if (!existing)
        {
            await _uow.Connections.Add(new Connection
            {
                UserFromId = link.UserId,
                UserToId = newUserId,
                ConnectionType = "referral",
                TrustScore = 10m,
                CreatedAt = DateTime.UtcNow
            });
            await _uow.CommitAsync();
        }
        return await _uow.Users.GetAsync(newUserId);
    }

    public async Task<IEnumerable<User>> GetReferralsAsync(long userId)
    {
        var referrals = (await _uow.Connections.GetAllAsync())
            .Where(c => c.UserFromId == userId && c.ConnectionType == "referral")
            .Select(c => c.UserToId)
            .ToList();
        var allUsers = await _uow.Users.GetAllAsync();
        return allUsers.Where(u => referrals.Contains(u.Id));
    }
}

public class InvoiceService : IInvoiceService
{
    private readonly IUnitOfWork _uow;

    public InvoiceService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Invoice?> CreateAsync(long? dealId, string number, decimal total, decimal platformFee, decimal taxRate)
    {
        var invoice = new Invoice
        {
            DealId = dealId,
            Number = number,
            Total = total,
            PlatformFee = platformFee,
            TaxRate = taxRate,
            Status = "draft"
        };
        await _uow.Invoices.Add(invoice);
        await _uow.CommitAsync();
        return invoice;
    }

    public async Task<Invoice?> MarkPaidAsync(long invoiceId)
    {
        var invoice = await _uow.Invoices.GetAsync(invoiceId);
        if (invoice == null)
        {
            return null;
        }
        invoice.Status = "paid";
        invoice.PaidAt = DateTime.UtcNow;
        _uow.Invoices.Update(invoice);
        await _uow.CommitAsync();
        return invoice;
    }

    public async Task<IEnumerable<Invoice>> GetAllAsync()
    {
        return await _uow.Invoices.GetAllAsync();
    }
}

public class WebhookService : IWebhookService
{
    private readonly IUnitOfWork _uow;

    public WebhookService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IEnumerable<WebhookSubscription>> GetByUserIdAsync(long userId)
    {
        return (await _uow.WebhookSubscriptions.GetAllAsync()).Where(w => w.UserId == userId && w.Active);
    }

    public async Task<WebhookSubscription?> SubscribeAsync(long userId, string url, string eventTypes, string? secret)
    {
        var sub = new WebhookSubscription
        {
            UserId = userId,
            Url = url,
            EventTypes = eventTypes,
            Secret = secret,
            Active = true
        };
        await _uow.WebhookSubscriptions.Add(sub);
        await _uow.CommitAsync();
        return sub;
    }

    public async Task<bool> UnsubscribeAsync(long subscriptionId, long userId)
    {
        var sub = await _uow.WebhookSubscriptions.GetAsync(subscriptionId);
        if (sub == null || sub.UserId != userId)
        {
            return false;
        }
        sub.Active = false;
        _uow.WebhookSubscriptions.Update(sub);
        await _uow.CommitAsync();
        return true;
    }

    // Постановка события в transactional outbox для доставки на внешние URL (A11 → A2)
    public async Task<int> DispatchEventAsync(string eventType, string? payloadJson, long? senderUserId = null)
    {
        var subscriptions = (await _uow.WebhookSubscriptions.GetAllAsync())
            .Where(w => w.Active && w.EventTypes.Split(',', StringSplitOptions.RemoveEmptyEntries).Contains(eventType))
            .ToList();
        if (subscriptions.Count == 0)
        {
            return 0;
        }
        foreach (var sub in subscriptions)
        {
            await _uow.OutboxMessages.Add(new OutboxMessage
            {
                EventType = eventType,
                AggregateId = sub.UserId.ToString(),
                PayloadJson = payloadJson ?? $"{{}}",
                Destination = sub.Url,
                CreatedAt = DateTime.UtcNow
            });
        }
        await _uow.CommitAsync();
        return subscriptions.Count;
    }
}

public class SkillService : ISkillService
{
    private readonly IUnitOfWork _uow;

    public SkillService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Skill?> GetSkillAsync(long id)
    {
        return await _uow.Skills.GetAsync(id);
    }

    public async Task<IEnumerable<SkillCategory>> GetCategoriesAsync()
    {
        return await _uow.SkillCategories.GetAllAsync();
    }

    public async Task<IEnumerable<Skill>> GetByCategoryAsync(long categoryId)
    {
        return await _uow.Skills.GetAllAsync().ContinueWith(t => t.Result.Where(s => s.CategoryId == categoryId));
    }

    public async Task<Skill?> AddSkillAsync(string name, long? parentId, long categoryId, string? description)
    {
        var skill = new Skill
        {
            Name = name,
            ParentId = parentId,
            CategoryId = categoryId,
            Description = description ?? string.Empty
        };
        await _uow.Skills.Add(skill);
        await _uow.CommitAsync();

        // Замыкающая таблица: добавляем путь ancestor→self и ancestor(предки)→self
        await _uow.SkillClosures.Add(new SkillClosure { AncestorId = skill.Id, DescendantId = skill.Id, Depth = 0 });
        await _uow.CommitAsync();
        return skill;
    }

    public async Task<UserSkill?> AddUserSkillAsync(long userId, long skillId, int level)
    {
        var userSkill = new UserSkill
        {
            UserId = userId,
            SkillId = skillId,
            Level = level,
            LastConfirmedAt = DateTime.UtcNow
        };
        await _uow.UserSkills.Add(userSkill);
        await _uow.CommitAsync();
        return userSkill;
    }

    public async Task<IEnumerable<UserSkill>> GetUserSkillsAsync(long userId)
    {
        return await _uow.UserSkills.GetAllAsync().ContinueWith(t => t.Result.Where(us => us.UserId == userId));
    }
}

public class MathService : IMathService
{
    // (3.2) нормировка «больше — лучше» с обязательным отсечением к [0..1].
    // Диссертация задаёт кусочную функцию с клампами: значения вне [min..max] не должны
    // давать xi > 1 или xi < 0, иначе взвешенная сумма (3.1) теряет смысл вероятностной меры.
    public decimal Normalize(decimal x, decimal min, decimal max)
    {
        if (max <= min)
        {
            // Вырожденный диапазон: требование либо выполнено, либо нет — без интерполяции.
            return x >= max ? 1m : 0m;
        }
        var t = (x - min) / (max - min);
        return t < 0m ? 0m : (t > 1m ? 1m : t);
    }

    // (3.3) нормировка «меньше — лучше» (например, время реакции, число ошибок).
    public decimal NormalizeDescending(decimal x, decimal min, decimal max)
    {
        return 1m - Normalize(x, min, max);
    }

    // (3.1) взвешенная сумма нормированных оценок: RoleFit = Σ(Vi · xi).
    // Веса Vi по смыслу образуют полную группу (Σ Vi = 1). Если сумма отличается,
    // веса приводятся к единице — иначе результат не сопоставим между ролями.
    public decimal RoleFit(decimal[] weights, decimal[] scores, decimal[] maxScores)
    {
        if (weights.Length != scores.Length || weights.Length != maxScores.Length || weights.Length == 0)
        {
            return 0m;
        }

        decimal weightSum = 0m;
        for (int i = 0; i < weights.Length; i++)
        {
            if (weights[i] < 0m)
            {
                return 0m;
            }
            weightSum += weights[i];
        }
        if (weightSum <= 0m)
        {
            return 0m;
        }

        decimal sum = 0m;
        for (int i = 0; i < weights.Length; i++)
        {
            decimal xi = Normalize(scores[i], 0m, maxScores[i]);
            sum += (weights[i] / weightSum) * xi;
        }
        return sum;
    }

    // (3.28) Кривая обучения: рост уровня при систематической практике.
    // F(t) = Fmax − (Fmax − F0)·e^(−k·t) — асимптотическое приближение к потолку Fmax.
    public decimal LearningCurve(decimal maxLevel, decimal startLevel, decimal learningRate, decimal days)
    {
        if (days <= 0m)
        {
            return startLevel;
        }
        var e = (decimal)Math.Exp((double)(-1m * learningRate * days));
        return maxLevel - (maxLevel - startLevel) * e;
    }

    // (3.29) Скорость освоения: сколько дней нужно, чтобы дойти от startLevel до targetLevel.
    // Обратная к (3.28): t = −ln((Fmax − Fцель)/(Fmax − F0)) / k.
    public decimal DaysToReach(decimal maxLevel, decimal startLevel, decimal targetLevel, decimal learningRate)
    {
        if (learningRate <= 0m || targetLevel <= startLevel)
        {
            return 0m;
        }
        if (targetLevel >= maxLevel)
        {
            return decimal.MaxValue;
        }
        var ratio = (double)((maxLevel - targetLevel) / (maxLevel - startLevel));
        return (decimal)(-Math.Log(ratio) / (double)learningRate);
    }

    // (3.30) Затухание компетенции без подтверждения: уровень ПАДАЕТ к остаточному floor.
    // F(t) = floor + (F0 − floor)·e^(−λ·t). Прежняя реализация ошибочно тянула уровень
    // ВВЕРХ к maxLevel — навык «рос» от бездействия.
    public decimal DecayedLevel(decimal currentLevel, DateTime lastConfirmedAt, decimal decayRate, DateTime now, decimal floorLevel = 0m)
    {
        var days = (decimal)(now - lastConfirmedAt).TotalDays;
        if (days <= 0m || decayRate <= 0m)
        {
            return currentLevel;
        }
        if (currentLevel <= floorLevel)
        {
            return currentLevel;
        }
        var e = (decimal)Math.Exp((double)(-1m * decayRate * days));
        return floorLevel + (currentLevel - floorLevel) * e;
    }

    [Obsolete("Семантически неверно: тянула уровень вверх к maxLevel. Используйте DecayedLevel (3.30) или LearningCurve (3.28).")]
    public decimal CurrentLevel(decimal maxLevel, decimal currentLevel, DateTime lastConfirmedAt, decimal decayRate, DateTime now)
    {
        return DecayedLevel(currentLevel, lastConfirmedAt, decayRate, now);
    }

    // Расстояние между координатами по формуле Гаверсина (км)
    public double HaversineDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double r = 6371.0;
        var dLat = (lat2 - lat1) * Math.PI / 180.0;
        var dLon = (lon2 - lon1) * Math.PI / 180.0;
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * Math.PI / 180.0) * Math.Cos(lat2 * Math.PI / 180.0) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return r * c;
    }

    // C4: кластеризация K-Means (k центров, случайные центроиды, до maxIterations итераций).
    // Возвращает массив меток кластера для каждой точки. Точки: вектор признаков (например, lat/long или навыки).
    public int[] KMeansCluster(double[][] points, int k, int maxIterations = 100)
    {
        if (points.Length == 0 || k <= 0)
        {
            return Array.Empty<int>();
        }
        if (k >= points.Length)
        {
            return Enumerable.Range(0, points.Length).ToArray();
        }

        int dim = points[0].Length;
        var labels = new int[points.Length];
        double[][] centroids = new double[k][];

        // Инициализация: k случайных точек данных как начальные центроиды (k-means++)
        var rng = new Random(DateTime.UtcNow.Millisecond);
        var chosen = new HashSet<int>();
        for (int c = 0; c < k; c++)
        {
            int idx;
            do
            {
                idx = rng.Next(points.Length);
            } while (!chosen.Add(idx));
            centroids[c] = (double[])points[idx].Clone();
        }

        for (int iter = 0; iter < maxIterations; iter++)
        {
            // Присваивание
            for (int i = 0; i < points.Length; i++)
            {
                double bestDist = double.MaxValue;
                int bestCluster = 0;
                for (int c = 0; c < k; c++)
                {
                    double dist = 0;
                    for (int d = 0; d < dim; d++)
                    {
                        dist += (points[i][d] - centroids[c][d]) * (points[i][d] - centroids[c][d]);
                    }
                    if (dist < bestDist)
                    {
                        bestDist = dist;
                        bestCluster = c;
                    }
                }
                labels[i] = bestCluster;
            }

            // Пересчёт центроидов (средние по точкам кластера)
            var newCentroids = new double[k][];
            var counts = new int[k];
            for (int c = 0; c < k; c++)
            {
                newCentroids[c] = new double[dim];
            }
            for (int i = 0; i < points.Length; i++)
            {
                int c = labels[i];
                counts[c]++;
                for (int d = 0; d < dim; d++)
                {
                    newCentroids[c][d] += points[i][d];
                }
            }
            bool converged = true;
            for (int c = 0; c < k; c++)
            {
                if (counts[c] == 0)
                {
                    // Пустой кластер в k-means — переинициализация случайной точкой (устойчивость)
                    centroids[c] = (double[])points[rng.Next(points.Length)].Clone();
                    converged = false;
                    continue;
                }
                for (int d = 0; d < dim; d++)
                {
                    newCentroids[c][d] /= counts[c];
                    if (Math.Abs(newCentroids[c][d] - centroids[c][d]) > 1e-9)
                    {
                        converged = false;
                    }
                }
                centroids[c] = newCentroids[c];
            }
            if (converged)
            {
                break;
            }
        }

        return labels;
    }

    // C4: задача о рюкзаке 0/1 методом ветвей и границ.
    // weights/values — веса и ценности элементов, capacity — вместимость.
    // Возвращает набор индексов с максимальной суммарной ценностью при ограничении веса.
    public KnapsackResult BranchAndBoundKnapsack(double[] weights, double[] values, double capacity)
    {
        int n = weights.Length;
        if (n == 0 || capacity <= 0)
        {
            return new KnapsackResult(0, 0, Array.Empty<int>());
        }

        // Сортируем по убыванию плотности ценности (value/weight)
        var items = Enumerable.Range(0, n)
            .Select(i => new { Index = i, Weight = weights[i], Value = values[i], Density = weights[i] > 0 ? values[i] / weights[i] : double.MaxValue })
            .OrderByDescending(x => x.Density)
            .ToArray();

        double bestValue = 0;
        var bestSet = new List<int>();
        var currentSet = new List<int>();

        void BoundAndBranch(int i, double curWeight, double curValue)
        {
            // Оптимистичная верхняя оценка — дробное (линейное) ослабление
            if (curValue > bestValue)
            {
                bestValue = curValue;
                bestSet = new List<int>(currentSet);
            }

            if (i >= n)
            {
                return;
            }

            // 1. Ветвь «включить» элемент
            double item = items[i].Weight;
            if (curWeight + item <= capacity)
            {
                currentSet.Add(items[i].Index);
                BoundAndBranch(i + 1, curWeight + item, curValue + items[i].Value);
                currentSet.RemoveAt(currentSet.Count - 1);
            }

            // 2. Ветвь «исключить» — используем дробное ослабление как верхнюю границу
            double remaining = capacity - curWeight;
            double upper = curValue;
            for (int j = i + 1; j < n; j++)
            {
                if (items[j].Weight <= remaining)
                {
                    upper += items[j].Value;
                    remaining -= items[j].Weight;
                }
                else
                {
                    upper += items[j].Density * remaining;
                    break;
                }
            }
            if (upper > bestValue)
            {
                BoundAndBranch(i + 1, curWeight, curValue);
            }
        }

        BoundAndBranch(0, 0, 0);

        double totalWeight = bestSet.Sum(idx => weights[idx]);
        return new KnapsackResult(bestValue, totalWeight, bestSet.OrderBy(x => x).ToArray());
    }
}

public class CompetenceService : ICompetenceService
{
    private readonly IUnitOfWork _uow;
    private readonly IMathService _math;

    public CompetenceService(IUnitOfWork uow, IMathService math)
    {
        _uow = uow;
        _math = math;
    }

    public async Task<IEnumerable<ProfessionalRole>> GetRolesAsync()
    {
        return await _uow.ProfessionalRoles.GetAllAsync();
    }

    // (3.1) RoleFit пользователя под роль
    public async Task<decimal> GetRoleFitAsync(long userId, long roleId)
    {
        var roleWeights = await _uow.RoleSkillWeights
            .GetAllAsync()
            .ContinueWith(t => t.Result.Where(w => w.RoleId == roleId).ToList());
        if (roleWeights.Count == 0)
        {
            return 0m;
        }

        var userSkills = await _uow.UserSkills
            .GetAllAsync()
            .ContinueWith(t => t.Result.Where(us => us.UserId == userId).ToDictionary(us => us.SkillId));

        var weights = new List<decimal>();
        var scores = new List<decimal>();
        var maxScores = new List<decimal>();

        foreach (var rw in roleWeights)
        {
            weights.Add(rw.Weight);
            maxScores.Add(rw.MaxScore);
            scores.Add(userSkills.TryGetValue(rw.SkillId, out var us) ? Math.Min(us.Level, 10) : 0m);
        }

        return _math.RoleFit(weights.ToArray(), scores.ToArray(), maxScores.ToArray());
    }

    // Кривая обучения/забывания: текущий уровень с учётом устаревания
    public async Task<decimal> GetCurrentLevelAsync(long userId, long skillId)
    {
        var userSkills = await _uow.UserSkills
            .GetAllAsync()
            .ContinueWith(t => t.Result.FirstOrDefault(us => us.UserId == userId && us.SkillId == skillId));
        if (userSkills == null)
        {
            return 0m;
        }
        // (3.30) уровень с учётом затухания: без подтверждения компетенция падает, а не растёт.
        return _math.DecayedLevel(userSkills.Level, userSkills.LastConfirmedAt, userSkills.DecayRate, DateTime.UtcNow);
    }
}

public class ConnectionService : IConnectionService
{
    private readonly IUnitOfWork _uow;

    public ConnectionService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IEnumerable<Connection>> GetByUserIdAsync(long userId)
    {
        return await _uow.Connections
            .GetAllAsync()
            .ContinueWith(t => t.Result.Where(c => c.UserFromId == userId || c.UserToId == userId));
    }

    public async Task<Connection?> CreateAsync(long userFromId, long userToId, string type)
    {
        var connection = new Connection
        {
            UserFromId = userFromId,
            UserToId = userToId,
            ConnectionType = type,
            TrustScore = 0m,
            CreatedAt = DateTime.UtcNow
        };
        await _uow.Connections.Add(connection);
        await _uow.CommitAsync();

        // Событие в брокер (connection.created -> аналитика сети)
        await _uow.OutboxMessages.Add(new OutboxMessage
        {
            EventType = "connection.created",
            AggregateId = connection.Id.ToString(),
            PayloadJson = System.Text.Json.JsonSerializer.Serialize(new { UserId = connection.UserFromId, connection.UserToId, connection.ConnectionType }),
            Destination = "kafka:analytics-events",
            CreatedAt = DateTime.UtcNow
        });
        await _uow.CommitAsync();
        return connection;
    }

    public async Task<decimal> GetTrustScoreAsync(long userFromId, long userToId)
    {
        var all = await _uow.Connections.GetAllAsync();
        var c = all.FirstOrDefault(x => x.UserFromId == userFromId && x.UserToId == userToId);
        return c?.TrustScore ?? 0m;
    }
}

public class ReputationService : IReputationService
{
    private readonly IUnitOfWork _uow;
    private readonly IMathService _math;

    public ReputationService(IUnitOfWork uow, IMathService math)
    {
        _uow = uow;
        _math = math;
    }

    public async Task<IEnumerable<ReputationEntry>> GetHistoryAsync(long userId, long skillId)
    {
        return await _uow.ReputationEntries
            .GetAllAsync()
            .ContinueWith(t => t.Result.Where(r => r.UserId == userId && r.SkillId == skillId));
    }

    public async Task<ReputationEntry> AddEntryAsync(long userId, long skillId, decimal value, string source)
    {
        var entry = new ReputationEntry
        {
            UserId = userId,
            SkillId = skillId,
            Value = value,
            Source = source,
            CreatedAt = DateTime.UtcNow
        };
        await _uow.ReputationEntries.Add(entry);
        await _uow.CommitAsync();
        return entry;
    }

    public async Task<ReputationScore> GetScoreAsync(long userId, long skillId)
    {
        var history = await GetHistoryAsync(userId, skillId);
        if (!history.Any())
        {
            return new ReputationScore(0m, "Novice");
        }
        var avg = history.Average(x => x.Value);
        var level = avg switch
        {
            >= 80m => "Master",
            >= 60m => "Expert",
            >= 40m => "Advanced",
            _ => "Novice"
        };
        return new ReputationScore(avg, level);
    }
}

public class SocialService : ISocialService
{
    private readonly IUnitOfWork _uow;
    private readonly IModerationService _moderation;

    public SocialService(IUnitOfWork uow, IModerationService moderation)
    {
        _uow = uow;
        _moderation = moderation;
    }

    public async Task<Post?> CreatePostAsync(long authorId, long? groupId, string body, string contentType, bool isPublic)
    {
        var moderation = await _moderation.ModerateTextAsync(body);
        var post = new Post
        {
            AuthorId = authorId,
            GroupId = groupId,
            Body = body,
            ContentType = contentType,
            IsPublic = isPublic,
            ModerationStatus = moderation.Status,
            ModerationReason = moderation.Reason,
            IsModerated = moderation.IsAllowed,
            CreatedAt = DateTime.UtcNow
        };
        await _uow.Posts.Add(post);
        await _uow.CommitAsync();

        // Событие в брокер (post.created -> feed-fanout подписчикам)
        await _uow.OutboxMessages.Add(new OutboxMessage
        {
            EventType = "post.created",
            AggregateId = post.Id.ToString(),
            PayloadJson = System.Text.Json.JsonSerializer.Serialize(post),
            Destination = "kafka:social-events",
            CreatedAt = DateTime.UtcNow
        });
        await _uow.CommitAsync();
        return post;
    }

    public async Task<IEnumerable<Post>> GetFeedAsync(long userId, int skip, int take)
    {
        var subs = await _uow.Subscriptions
            .GetAllAsync()
            .ContinueWith(t => t.Result.Where(s => s.UserId == userId && s.TargetType == "user").Select(s => s.TargetUserId).ToHashSet());
        var posts = await _uow.Posts.GetAllAsync();
        return posts.Where(p => subs.Contains(p.AuthorId) && p.IsPublic && p.ModerationStatus != "rejected").OrderByDescending(p => p.CreatedAt).Skip(skip).Take(take);
    }

    public async Task<Group?> CreateGroupAsync(long ownerId, string name, bool isPublic)
    {
        var group = new Group { OwnerId = ownerId, Name = name, IsPublic = isPublic };
        await _uow.Groups.Add(group);
        await _uow.CommitAsync();
        await _uow.Memberships.Add(new Membership { GroupId = group.Id, UserId = ownerId, Role = "owner", JoinedAt = DateTime.UtcNow });
        await _uow.CommitAsync();
        return group;
    }

    public async Task<Membership?> JoinGroupAsync(long groupId, long userId)
    {
        var membership = new Membership { GroupId = groupId, UserId = userId, Role = "member", JoinedAt = DateTime.UtcNow };
        await _uow.Memberships.Add(membership);
        await _uow.CommitAsync();
        return membership;
    }

    public async Task<Forum?> CreateForumAsync(long groupId, string name)
    {
        var forum = new Forum { GroupId = groupId, Name = name };
        await _uow.Forums.Add(forum);
        await _uow.CommitAsync();
        return forum;
    }

    public async Task<ForumTopic?> CreateTopicAsync(long categoryId, long authorId, string title)
    {
        var topic = new ForumTopic { CategoryId = categoryId, AuthorId = authorId, Title = title, CreatedAt = DateTime.UtcNow };
        await _uow.ForumTopics.Add(topic);
        await _uow.CommitAsync();
        return topic;
    }

    public async Task<Subscription?> SubscribeAsync(long userId, long targetUserId, string targetType)
    {
        var sub = new Subscription { UserId = userId, TargetUserId = targetUserId, TargetType = targetType, CreatedAt = DateTime.UtcNow };
        await _uow.Subscriptions.Add(sub);
        await _uow.CommitAsync();
        return sub;
    }
}

public class DealService : IDealService
{
    private readonly IUnitOfWork _uow;

    public DealService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Deal?> CreateAsync(long clientId, long contractorId, decimal amount, string? contractTerms)
    {
        var deal = new Deal
        {
            ClientId = clientId,
            ContractorId = contractorId,
            Amount = amount,
            ContractTerms = contractTerms,
            Status = "created"
        };
        await _uow.Deals.Add(deal);
        await _uow.CommitAsync();
        return deal;
    }

    public async Task<Deal?> EscrowAsync(long dealId, decimal amount)
    {
        var deal = await _uow.Deals.GetAsync(dealId);
        if (deal == null)
        {
            return null;
        }
        deal.EscrowHeld = amount;
        deal.Status = "escrowed";
        _uow.Deals.Update(deal);
        await _uow.CommitAsync();
        return deal;
    }

    public async Task<Deal?> AddMilestoneAsync(long dealId, string title, decimal amount)
    {
        var deal = await _uow.Deals.GetAsync(dealId);
        if (deal == null)
        {
            return null;
        }
        await _uow.DealMilestones.Add(new DealMilestone { DealId = dealId, Title = title, Amount = amount, Status = "pending" });
        await _uow.CommitAsync();
        return deal;
    }

    public async Task<Deal?> CompleteMilestoneAsync(long milestoneId)
    {
        var milestone = await _uow.DealMilestones.GetAsync(milestoneId);
        if (milestone == null)
        {
            return null;
        }
        milestone.Status = "completed";
        milestone.CompletedAt = DateTime.UtcNow;
        _uow.DealMilestones.Update(milestone);
        await _uow.CommitAsync();
        return await _uow.Deals.GetAsync(milestone.DealId);
    }

    public async Task<Deal?> CompleteAsync(long dealId)
    {
        var deal = await _uow.Deals.GetAsync(dealId);
        if (deal == null)
        {
            return null;
        }
        deal.Status = "completed";
        deal.EscrowHeld = 0m;
        _uow.Deals.Update(deal);
        await _uow.CommitAsync();

        // Событие в брокер (deal.completed -> пересчёт репутации контрагентов)
        await _uow.OutboxMessages.Add(new OutboxMessage
        {
            EventType = "deal.completed",
            AggregateId = deal.Id.ToString(),
            PayloadJson = System.Text.Json.JsonSerializer.Serialize(deal),
            Destination = "kafka:deal-events",
            CreatedAt = DateTime.UtcNow
        });
        await _uow.CommitAsync();
        return deal;
    }

    public async Task<Deal?> DisputeAsync(long dealId)
    {
        var deal = await _uow.Deals.GetAsync(dealId);
        if (deal == null)
        {
            return null;
        }
        deal.Status = "disputed";
        _uow.Deals.Update(deal);
        await _uow.CommitAsync();
        return deal;
    }
}

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _uow;

    public NotificationService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Notification> CreateAsync(long userId, string type, string payloadJson, string channel)
    {
        var notification = new Notification
        {
            UserId = userId,
            Type = type,
            PayloadJson = payloadJson,
            Channel = channel,
            CreatedAt = DateTime.UtcNow
        };
        await _uow.Notifications.Add(notification);
        await _uow.CommitAsync();

        // Transactional Outbox: событие для внешней доставки (все в одной транзакции)
        await _uow.OutboxMessages.Add(new OutboxMessage
        {
            EventType = "notification.created",
            AggregateId = notification.UserId.ToString(),
            PayloadJson = System.Text.Json.JsonSerializer.Serialize(notification),
            Destination = $"signalr:user-{notification.UserId}",
            CreatedAt = DateTime.UtcNow
        });
        await _uow.CommitAsync();
        return notification;
    }

    public async Task<IEnumerable<Notification>> GetUnreadAsync(long userId)
    {
        return await _uow.Notifications
            .GetAllAsync()
            .ContinueWith(t => t.Result.Where(n => n.UserId == userId && !n.Read).OrderByDescending(n => n.CreatedAt));
    }

    public async Task<IEnumerable<Notification>> GetAllAsync(long userId)
    {
        return await _uow.Notifications
            .GetAllAsync()
            .ContinueWith(t => t.Result.Where(n => n.UserId == userId).OrderByDescending(n => n.CreatedAt));
    }

    public async Task MarkReadAsync(long notificationId)
    {
        var notification = await _uow.Notifications.GetAsync(notificationId);
        if (notification != null)
        {
            notification.Read = true;
            _uow.Notifications.Update(notification);
            await _uow.CommitAsync();
        }
    }
}

public class MessageService : IMessageService
{
    private readonly IUnitOfWork _uow;

    public MessageService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IEnumerable<Message>> GetConversationAsync(long conversationId)
    {
        return await _uow.Messages
            .GetAllAsync()
            .ContinueWith(t => t.Result.Where(m => m.ConversationId == conversationId).OrderBy(m => m.CreatedAt));
    }

    public async Task<Message> SendAsync(long conversationId, long senderId, string body)
    {
        var message = new Message
        {
            ConversationId = conversationId,
            SenderId = senderId,
            Body = body,
            CreatedAt = DateTime.UtcNow,
            Delivered = true
        };
        await _uow.Messages.Add(message);
        await _uow.CommitAsync();
        return message;
    }

    public async Task<Conversation?> CreateConversationAsync(long userIdA, long userIdB)
    {
        var conversation = new Conversation { Type = "dm", Title = $"dm-{userIdA}-{userIdB}" };
        await _uow.Conversations.Add(conversation);
        await _uow.CommitAsync();
        await _uow.ConversationMembers.Add(new ConversationMember { ConversationId = conversation.Id, UserId = userIdA, JoinedAt = DateTime.UtcNow });
        await _uow.ConversationMembers.Add(new ConversationMember { ConversationId = conversation.Id, UserId = userIdB, JoinedAt = DateTime.UtcNow });
        await _uow.CommitAsync();
        return conversation;
    }
}

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _uow;
    private readonly JwtOptions _jwt;

    public AuthService(IUnitOfWork uow, JwtOptions jwt)
    {
        _uow = uow;
        _jwt = jwt;
    }

    public async Task<AuthResult?> RegisterAsync(string email, string name, string password)
    {
        var users = await _uow.Users.GetAllAsync();
        if (users.Any(u => u.Email == email))
        {
            return null;
        }

        var user = new User
        {
            Email = email,
            Name = name,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = UserRoles.User,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        await _uow.Users.Add(user);
        await _uow.CommitAsync();
        return CreateToken(user);
    }

    public async Task<AuthResult?> LoginAsync(string email, string password)
    {
        var users = await _uow.Users.GetAllAsync();
        var user = users.FirstOrDefault(u => u.Email == email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return null;
        }
        if (!user.IsActive)
        {
            return null;
        }
        return CreateToken(user);
    }

    public async Task<User?> GetByIdAsync(long id)
    {
        return await _uow.Users.GetAsync(id);
    }

    private AuthResult CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_jwt.ExpiryMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expires,
            signingCredentials: creds);

        return new AuthResult(
            new JwtSecurityTokenHandler().WriteToken(token),
            user.Id, user.Email, user.Name, user.Role);
    }
}

public class SearchService : ISearchService
{
    private readonly IUnitOfWork _uow;
    private readonly ISearchRepository _search;

    public SearchService(IUnitOfWork uow, ISearchRepository search)
    {
        _uow = uow;
        _search = search;
    }

    public async Task<IEnumerable<SearchHit>> SearchAsync(string query, string type, int skip, int take)
    {
        var candidates = await _search.SearchAsync(query, type, skip, take);
        return candidates.Select(c => c.Type == "hashtag"
            ? new SearchHit(c.Id, c.Type, "#" + c.Title, $"{c.Subtitle} постов", (decimal)c.Score)
            : new SearchHit(c.Id, c.Type, c.Title, c.Subtitle, (decimal)c.Score));
    }

    public async Task<Hashtag?> GetOrAddHashtagAsync(string tag)
    {
        var tagName = tag.TrimStart('#').ToLowerInvariant();
        var hashtags = await _uow.Hashtags.GetAllAsync();
        var existing = hashtags.FirstOrDefault(h => h.Tag == tagName);
        if (existing != null)
        {
            return existing;
        }
        var created = new Hashtag { Tag = tagName, UsageCount = 0, CreatedAt = DateTime.UtcNow };
        await _uow.Hashtags.Add(created);
        await _uow.CommitAsync();
        return created;
    }

    public async Task<IEnumerable<SearchHit>> TrendingHashtagsAsync(int take)
    {
        var hashtags = await _uow.Hashtags.GetAllAsync();
        return hashtags
            .OrderByDescending(h => h.UsageCount)
            .Take(take)
            .Select(h => new SearchHit(h.Id, "hashtag", "#" + h.Tag, $"{h.UsageCount} постов", h.UsageCount));
    }
}