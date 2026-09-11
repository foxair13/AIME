namespace AI.SocialNetwork.Application.Contracts;

public interface IEventDispatcher
{
    // Доменный обработчик события из Outbox/брокера (eventType: payloadJson)
    Task DispatchAsync(string eventType, string payloadJson, CancellationToken ct = default);
}