using AI.SocialNetwork.Application.Contracts;
using Confluent.Kafka;

namespace AI.SocialNetwork.Web.Services;

// Фоновый потребитель событий: подписка на топики брокера и диспатч в EventDispatcher.
// Запускается только когда брокер доступен; иначе логирует и ждёт следующего тика проверки.
public class KafkaConsumerService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _config;
    private readonly ILogger<KafkaConsumerService> _logger;
    private const int RetrySeconds = 30;

    public KafkaConsumerService(IServiceScopeFactory scopeFactory, IConfiguration config, ILogger<KafkaConsumerService> logger)
    {
        _scopeFactory = scopeFactory;
        _config = config;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var publisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();
                    if (publisher is not KafkaEventPublisher kafka || !kafka.IsAvailable)
                    {
                        _logger.LogInformation("Kafka-консьюмер: брокер недоступен — повтор через {Retry}s", RetrySeconds);
                        await Task.Delay(TimeSpan.FromSeconds(RetrySeconds), stoppingToken);
                        continue;
                    }
                }

                await ConsumeUntilStoppedAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kafka-консьюмер: ошибка цикла — повтор через {Retry}s", RetrySeconds);
                await Task.Delay(TimeSpan.FromSeconds(RetrySeconds), stoppingToken);
            }
        }
    }

    private async Task ConsumeUntilStoppedAsync(CancellationToken stoppingToken)
    {
        var bootstrap = _config["Kafka:BootstrapServers"] ?? "localhost:9092";
        var groupId = _config["Kafka:ConsumerGroupId"] ?? "ai-social-network-consumer";
        var topics = GetTopics();

        _logger.LogInformation("Kafka-консьюмер запущен: group {Group}, topics [{Topics}]", groupId, string.Join(", ", topics));

        using var consumer = new ConsumerBuilder<string, string>(new ConsumerConfig
        {
            BootstrapServers = bootstrap,
            GroupId = groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true,
            AllowAutoCreateTopics = false
        }).Build();

        try
        {
            consumer.Subscribe(topics);
            while (!stoppingToken.IsCancellationRequested)
            {
                var cr = consumer.Consume(TimeSpan.FromSeconds(1));
                if (cr == null)
                {
                    continue;
                }

                await DispatchAsync(cr.Message.Key, cr.Message.Value, stoppingToken);
            }
        }
        catch (ConsumeException ex)
        {
            _logger.LogWarning(ex, "Kafka-консьюмер: ошибка потребления ({Status})", ex.Error.Reason);
        }
        finally
        {
            consumer.Close();
        }
    }

    private async Task DispatchAsync(string eventType, string payloadJson, CancellationToken ct)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var dispatcher = scope.ServiceProvider.GetRequiredService<IEventDispatcher>();
            await dispatcher.DispatchAsync(eventType, payloadJson, ct);
        }
        catch (Exception ex)
        {
            // Не валим консьюмер: сообщение будет обработано повторно при реплее/последующем чтении.
            _logger.LogError(ex, "Kafka-консьюмер: сбой обработки {EventType}", eventType);
        }
    }

    private string[] GetTopics()
    {
        var configured = _config.GetSection("Kafka:Topics").Get<string[]>();
        return configured is { Length: > 0 }
            ? configured
            : new[] { "social-events", "deal-events", "analytics-events", "agent-events" };
    }
}