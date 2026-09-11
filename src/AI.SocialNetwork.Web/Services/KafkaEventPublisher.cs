using Confluent.Kafka;

namespace AI.SocialNetwork.Web.Services;

// Реализация публикатора событий поверх Confluent.Kafka.
// При недоступности брокера (нет Docker/localhost:9092) методы-публикации возвращают false —
// потребитель должен переключиться на локальную обработку (fallback), как у Ollama.
public class KafkaEventPublisher : IEventPublisher, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaEventPublisher> _logger;
    private volatile bool _isConnected;

    public KafkaEventPublisher(IConfiguration config, ILogger<KafkaEventPublisher> logger)
    {
        _logger = logger;
        var bootstrapServers = config["Kafka:BootstrapServers"] ?? "localhost:9092";
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
            LingerMs = 5,
            MessageTimeoutMs = 5000,
            EnableIdempotence = false,
            Acks = Acks.All
        };

        try
        {
            _producer = new ProducerBuilder<string, string>(producerConfig).Build();
            _isConnected = true; // предполагаем доступность; реальная проверка при ProduceAsync
            _logger.LogInformation("Kafka: продюсер создан, bootstrap {Servers}", bootstrapServers);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Kafka: не удалось собрать продюсер — используется fallback-режим");
            _producer = null!;
            _isConnected = false;
        }
    }

    public bool IsAvailable => _isConnected;
    public string? Status => _isConnected ? "connected" : "unavailable";

    public async Task<bool> PublishAsync(string topic, string key, string payloadJson, CancellationToken ct = default)
    {
        if (_producer == null || !_isConnected)
        {
            return false;
        }

        try
        {
            await _producer.ProduceAsync(topic, new Message<string, string>
            {
                Key = key,
                Value = payloadJson
            }, ct);
            return true;
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogWarning(ex, "Kafka: публикация в {Topic} не удалась ({Status})", topic, ex.Error.Reason);
            _isConnected = false;
            return false;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
    }

    public void Dispose() => _producer?.Dispose();
}