using AI.SocialNetwork.Application.Contracts;
using AI.SocialNetwork.Domain.Entities;
using AI.SocialNetwork.Infrastructure.DataContext;
using Microsoft.EntityFrameworkCore;

namespace AI.SocialNetwork.Web.Services;

// Фоновая обработка Transactional Outbox: доставка отложенных событий.
// В полном решении Kafka/webhooks/push/email; в этой версии:
//   - destination "kafka:<topic>" -> публикация в брокер (Kafka) или локальный fallback-диспетчер;
//   - destination "signalr:user-N" -> in-app доставка через hub (помечается доставленным);
//   - destination http(s)          -> webhook по HTTP.
public class OutboxProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxProcessor> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IEventPublisher _publisher;

    public OutboxProcessor(
        IServiceScopeFactory scopeFactory,
        ILogger<OutboxProcessor> logger,
        IHttpClientFactory httpClientFactory,
        IEventPublisher publisher)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _publisher = publisher;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OutboxProcessor запущен");
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(10));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await ProcessPendingAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка обработки Outbox");
            }
        }
    }

    private async Task ProcessPendingAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AISocialNetworkContext>();

        var pending = await context.OutboxMessages
            .Where(m => m.Status == "pending" && m.ProcessedAt == null)
            .OrderBy(m => m.CreatedAt)
            .Take(50)
            .ToListAsync(ct);

        foreach (var message in pending)
        {
            var ok = await DeliverAsync(message, ct);
            message.ProcessedAt = DateTime.UtcNow;
            if (ok)
            {
                message.Status = "delivered";
            }
            else
            {
                message.RetryCount++;
                message.Status = message.RetryCount >= 5 ? "failed" : "pending";
                message.LastError = $"retry {message.RetryCount}";
            }
        }

        if (pending.Count > 0)
        {
            await context.SaveChangesAsync(ct);
        }
    }

    private async Task<bool> DeliverAsync(OutboxMessage message, CancellationToken ct)
    {
        // Kafka-доставка: destination = "kafka:<topic>"
        if (message.Destination is not null && message.Destination.StartsWith("kafka:", StringComparison.OrdinalIgnoreCase))
        {
            var topic = message.Destination["kafka:".Length..];
            var ok = await _publisher.PublishAsync(topic, message.EventType, message.PayloadJson, ct);
            if (ok)
            {
                _logger.LogInformation("Outbox {Event} -> kafka:{Topic} (delivered)", message.EventType, topic);
                return true;
            }

            // Fallback: брокер недоступен — обрабатываем локально тем же диспетчером.
            _logger.LogWarning("Outbox {Event} -> kafka:{Topic} недоступен; локальная обработка",
                message.EventType, topic);
            using var scope = _scopeFactory.CreateScope();
            var dispatcher = scope.ServiceProvider.GetRequiredService<IEventDispatcher>();
            await dispatcher.DispatchAsync(message.EventType, message.PayloadJson, ct);
            return true;
        }

        // signalr:user-N метки обрабатываются только через хаб (in-app); здесь логируем как доставленные.
        if (message.Destination is not null && message.Destination.StartsWith("signalr:", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInformation("Outbox {Event} -> {Destination} (in-app via SignalR)", message.EventType, message.Destination);
            return true;
        }

        // Внешние webhook-адреса доставляются по HTTP (безопасность: HTTP-клиент короткого таймаута).
        if (message.Destination is not null && message.Destination.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                using var client = _httpClientFactory.CreateClient("outbox");
                using var content = new StringContent(message.PayloadJson, System.Text.Encoding.UTF8, "application/json");
                var response = await client.PostAsync(message.Destination, content, ct);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Доставка webhook {Dest} не удалась", message.Destination);
                return false;
            }
        }

        _logger.LogInformation("Outbox {Event} @ {Destination} — пропуск", message.EventType, message.Destination);
        return true;
    }
}