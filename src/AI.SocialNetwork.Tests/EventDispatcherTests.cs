using System.Text.Json;
using AI.SocialNetwork.Application.Contracts;
using AI.SocialNetwork.Application.Services;
using AI.SocialNetwork.Domain.Entities;
using AI.SocialNetwork.Infrastructure.Repositories.Contracts;
using AI.SocialNetwork.Infrastructure.UnitOfWork.Contracts;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace AI.SocialNetwork.Tests;

public class EventDispatcherTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IGenericRepository<Subscription>> _subscriptions = new();
    private readonly Mock<IGenericRepository<Notification>> _notifications = new();
    private readonly Mock<IGenericRepository<AnalyticsEvent>> _analytics = new();
    private readonly Mock<IGenericRepository<OutboxMessage>> _outbox = new();
    private readonly Mock<IGenericRepository<Skill>> _skills = new();
    private readonly Mock<IReputationService> _reputation = new();

    private EventDispatcher CreateDispatcher()
    {
        _uow.Setup(u => u.Subscriptions).Returns(_subscriptions.Object);
        _uow.Setup(u => u.Notifications).Returns(_notifications.Object);
        _uow.Setup(u => u.AnalyticsEvents).Returns(_analytics.Object);
        _uow.Setup(u => u.OutboxMessages).Returns(_outbox.Object);
        _uow.Setup(u => u.Skills).Returns(_skills.Object);
        _skills.Setup(r => r.GetAllAsync()).ReturnsAsync([
            new Skill { Id = 1, Name = "Dealing", CategoryId = 1 }
        ]);
        return new EventDispatcher(_uow.Object, _reputation.Object, NullLogger<EventDispatcher>.Instance);
    }

    [Fact]
    public async Task PostCreated_NotifiesSubscribersOfAuthor_FanOut()
    {
        _subscriptions.Setup(r => r.GetAllAsync()).ReturnsAsync([
            new Subscription { UserId = 2, TargetUserId = 7, TargetType = "user" },
            new Subscription { UserId = 3, TargetUserId = 7, TargetType = "user" },
            new Subscription { UserId = 9, TargetUserId = 2, TargetType = "user" }, // чужой подписчик
        ]);

        var post = new Post { Id = 15, AuthorId = 7, Body = "новый пост", IsPublic = true };

        var dispatcher = CreateDispatcher();
        await dispatcher.DispatchAsync("post.created", JsonSerializer.Serialize(post));

        _notifications.Verify(r => r.Add(It.Is<Notification>(n => n.UserId == 2 && n.Type == "feed")), Times.Once);
        _notifications.Verify(r => r.Add(It.Is<Notification>(n => n.UserId == 3 && n.Type == "feed")), Times.Once);
        _notifications.Verify(r => r.Add(It.Is<Notification>(n => n.UserId == 9)), Times.Never);
    }

    [Fact]
    public async Task DealCompleted_AddsReputationToBothParties()
    {
        var deal = new Deal { Id = 4, ClientId = 10, ContractorId = 11, Amount = 2500m };

        var dispatcher = CreateDispatcher();
        await dispatcher.DispatchAsync("deal.completed", JsonSerializer.Serialize(deal));

        _reputation.Verify(r => r.AddEntryAsync(10, 1, It.Is<decimal>(v => v > 0 && v <= 100), "deal"), Times.Once);
        _reputation.Verify(r => r.AddEntryAsync(11, 1, It.Is<decimal>(v => v > 0 && v <= 100), "deal"), Times.Once);
    }

    [Fact]
    public async Task UserRegistered_WritesAnalyticsEvent()
    {
        var payload = JsonSerializer.Serialize(new { UserId = 42, Email = "new@user.ru" });

        var dispatcher = CreateDispatcher();
        await dispatcher.DispatchAsync("user.registered", payload);

        _analytics.Verify(r => r.Add(It.Is<AnalyticsEvent>(e =>
            e.UserId == 42 && e.EventType == "user.registered")), Times.Once);
    }

    [Fact]
    public async Task UnknownEvent_IsIgnored()
    {
        var dispatcher = CreateDispatcher();
        await dispatcher.DispatchAsync("something.else", "{}");

        _analytics.Verify(r => r.Add(It.IsAny<AnalyticsEvent>()), Times.Never);
        _notifications.Verify(r => r.Add(It.IsAny<Notification>()), Times.Never);
        _reputation.Verify(r => r.AddEntryAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<decimal>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task DealCompleted_SameParty_ReputationOnce()
    {
        var deal = new Deal { Id = 4, ClientId = 10, ContractorId = 10, Amount = 500m };

        var dispatcher = CreateDispatcher();
        await dispatcher.DispatchAsync("deal.completed", JsonSerializer.Serialize(deal));

        _reputation.Verify(r => r.AddEntryAsync(10, 1, It.IsAny<decimal>(), "deal"), Times.Once);
    }
}