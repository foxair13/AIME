namespace AI.SocialNetwork.Web.Services;

public interface IEventPublisher
{
    // Публикует событие в Kafka-топик. false — брокер недоступен (нужен fallback).
    Task<bool> PublishAsync(string topic, string key, string payloadJson, CancellationToken ct = default);
    string? Status { get; }
}