using System.Text.Json;
using AI.SocialNetwork.Application.Contracts;
using AI.SocialNetwork.Domain.Entities;
using AI.SocialNetwork.Infrastructure.UnitOfWork.Contracts;
using Microsoft.Extensions.Logging;

namespace AI.SocialNetwork.Application.Services;

// Обработчик доменных событий, доставленных через Outbox/брокер (Kafka).
// Задачи, ради которых подключён брокер:
//   post.created        -> feed-fanout: уведомить подписчиков автора
//   deal.completed      -> пересчёт репутации контрагентов
//   connection.created  -> запись в аналитику сети
//   user.registered     -> запись в аналитику регистраций
//   agent.message.sent  -> запись в аналитику трафика агентов
public class EventDispatcher : IEventDispatcher
{
    private readonly IUnitOfWork _uow;
    private readonly IReputationService _reputation;
    private readonly ILogger<EventDispatcher> _logger;

    public EventDispatcher(IUnitOfWork uow, IReputationService reputation, ILogger<EventDispatcher> logger)
    {
        _uow = uow;
        _reputation = reputation;
        _logger = logger;
    }

    public async Task DispatchAsync(string eventType, string payloadJson, CancellationToken ct = default)
    {
        _logger.LogInformation("Обработка события {EventType}", eventType);
        switch (eventType)
        {
            case "post.created":
                await HandlePostCreatedAsync(payloadJson);
                break;
            case "deal.completed":
                await HandleDealCompletedAsync(payloadJson);
                break;
            case "connection.created":
                await LogAnalyticsAsync(eventType, payloadJson);
                break;
            case "user.registered":
                await LogAnalyticsAsync(eventType, payloadJson);
                break;
            case "agent.message.sent":
                await LogAnalyticsAsync(eventType, payloadJson);
                break;
            default:
                _logger.LogDebug("Нет обработчика для события {EventType} — пропускаем", eventType);
                break;
        }
    }

    // Feed-fanout: уведомляем подписчиков автора о новом посте.
    private async Task HandlePostCreatedAsync(string payloadJson)
    {
        var post = JsonSerializer.Deserialize<Post>(payloadJson, JsonOpts());
        if (post == null || post.AuthorId == 0)
        {
            return;
        }

        var subscribers = (await _uow.Subscriptions.GetAllAsync())
            .Where(s => s.TargetType == "user" && s.TargetUserId == post.AuthorId)
            .Select(s => s.UserId)
            .Distinct()
            .ToList();

        foreach (var subscriberId in subscribers)
        {
            await _uow.Notifications.Add(new Notification
            {
                UserId = subscriberId,
                Type = "feed",
                Channel = "in_app",
                PayloadJson = JsonSerializer.Serialize(post),
                CreatedAt = DateTime.UtcNow
            });
        }

        if (subscribers.Count > 0)
        {
            await _uow.CommitAsync();
            _logger.LogInformation("post.created: {Count} подписчиков уведомлены", subscribers.Count);
        }
    }

    // Пересчёт репутации контрагентов по закрытой сделке.
    private async Task HandleDealCompletedAsync(string payloadJson)
    {
        var deal = JsonSerializer.Deserialize<Deal>(payloadJson, JsonOpts());
        if (deal == null || (deal.ClientId == 0 && deal.ContractorId == 0))
        {
            return;
        }

        // Демонстрационная математика: доля репутации по логарифму суммы сделки (кэп 100).
        var contribution = Math.Min(100m, 1m + (decimal)Math.Log10((double)Math.Max(1m, deal.Amount)) * 10m);

        // Репутация привязывается к реальному навыку (SkillId). Берём первый из списка;
        // если навыков нет — пропускаем пересчёт, чтобы не нарушить FK.
        var skills = await _uow.Skills.GetAllAsync();
        var skill = skills.FirstOrDefault();
        if (skill != null)
        {
            if (deal.ClientId > 0)
            {
                await _reputation.AddEntryAsync(deal.ClientId, skill.Id, contribution, "deal");
            }

            if (deal.ContractorId > 0 && deal.ContractorId != deal.ClientId)
            {
                await _reputation.AddEntryAsync(deal.ContractorId, skill.Id, contribution, "deal");
            }
        }

        _logger.LogInformation("deal.completed: репутация пересчитана для deal {DealId}", deal.Id);
    }

    // Запись события в хранилище аналитики.
    private async Task LogAnalyticsAsync(string eventType, string payloadJson)
    {
        long? userId = null;
        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            if (doc.RootElement.TryGetProperty("UserId", out var u) && u.TryGetInt64(out var id))
            {
                userId = id;
            }
        }
        catch (JsonException)
        {
            // неструктурированный payload — UserId будет null
        }

        await _uow.AnalyticsEvents.Add(new AnalyticsEvent
        {
            UserId = userId,
            EventType = eventType,
            PropertiesJson = payloadJson,
            CreatedAt = DateTime.UtcNow
        });
        await _uow.CommitAsync();
    }

    private static System.Text.Json.JsonSerializerOptions JsonOpts()
        => new() { PropertyNameCaseInsensitive = true };
}