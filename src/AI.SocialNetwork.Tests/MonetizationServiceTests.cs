using AI.SocialNetwork.Application.Services;
using AI.SocialNetwork.Domain.Entities;
using AI.SocialNetwork.Infrastructure.Repositories.Contracts;
using AI.SocialNetwork.Infrastructure.UnitOfWork.Contracts;
using Moq;

namespace AI.SocialNetwork.Tests;

public class MonetizationServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IGenericRepository<ReferralLink>> _links = new();
    private readonly Mock<IGenericRepository<Connection>> _connections = new();
    private readonly Mock<IGenericRepository<User>> _users = new();
    private readonly Mock<IGenericRepository<Invoice>> _invoices = new();
    private readonly Mock<IGenericRepository<WebhookSubscription>> _webhooks = new();
    private readonly Mock<IGenericRepository<OutboxMessage>> _outbox = new();

    public MonetizationServiceTests()
    {
        _uow.Setup(u => u.ReferralLinks).Returns(_links.Object);
        _uow.Setup(u => u.Connections).Returns(_connections.Object);
        _uow.Setup(u => u.Users).Returns(_users.Object);
        _uow.Setup(u => u.Invoices).Returns(_invoices.Object);
        _uow.Setup(u => u.WebhookSubscriptions).Returns(_webhooks.Object);
        _uow.Setup(u => u.OutboxMessages).Returns(_outbox.Object);
    }

    [Fact]
    public async Task GetOrCreate_CreatesEightCharCode()
    {
        _links.Setup(r => r.GetAllAsync()).ReturnsAsync(Array.Empty<ReferralLink>());

        var service = new ReferralService(_uow.Object);
        var link = await service.GetOrCreateAsync(10);

        Assert.NotNull(link);
        Assert.Equal(8, link!.Code.Length);
        Assert.True(link.Code.All(char.IsAsciiLetterUpper));
    }

    [Fact]
    public async Task ApplyCode_UnknownCode_ReturnsNull()
    {
        _links.Setup(r => r.GetAllAsync()).ReturnsAsync([
            new ReferralLink { Id = 1, UserId = 10, Code = "ABCDEFGH" }
        ]);

        var service = new ReferralService(_uow.Object);
        var result = await service.ApplyCodeAsync("NOPE1234", 99);
        Assert.Null(result);
    }

    [Fact]
    public async Task ApplyCode_ValidCode_CreatesReferralConnection()
    {
        _links.Setup(r => r.GetAllAsync()).ReturnsAsync([
            new ReferralLink { Id = 1, UserId = 10, Code = "ABCDEFGH" }
        ]);
        _connections.Setup(r => r.GetAllAsync()).ReturnsAsync(Array.Empty<Connection>());
        var newUser = new User { Id = 99, Email = "new@user.ru", Name = "Новый" };
        _users.Setup(r => r.GetAsync(99L)).ReturnsAsync(newUser);

        var service = new ReferralService(_uow.Object);
        var result = await service.ApplyCodeAsync("ABCDEFGH", 99);

        Assert.NotNull(result);
        _connections.Verify(r => r.Add(It.Is<Connection>(c =>
            c.UserFromId == 10 && c.UserToId == 99 && c.ConnectionType == "referral")), Times.Once);
    }

    [Fact]
    public async Task MarkPaid_SetsStatusAndDate()
    {
        var invoice = new Invoice { Id = 5, Status = "draft" };
        _invoices.Setup(r => r.GetAsync(5L)).ReturnsAsync(invoice);

        var service = new InvoiceService(_uow.Object);
        var result = await service.MarkPaidAsync(5);

        Assert.NotNull(result);
        Assert.Equal("paid", result!.Status);
        Assert.NotNull(result.PaidAt);
    }

    [Fact]
    public async Task Subscribe_CreatesActiveSubscription()
    {
        WebhookSubscription? saved = null;
        _webhooks.Setup(r => r.Add(It.IsAny<WebhookSubscription>())).Callback<WebhookSubscription>(w => saved = w);

        var service = new WebhookService(_uow.Object);
        var result = await service.SubscribeAsync(10, "https://hook.example/event", "deal.completed,user.registered", "sec");

        Assert.NotNull(result);
        Assert.True(result!.Active);
        Assert.Equal("https://hook.example/event", saved!.Url);
    }

    [Fact]
    public async Task DispatchEvent_MatchingSubscription_WritesOutbox()
    {
        _webhooks.Setup(r => r.GetAllAsync()).ReturnsAsync([
            new WebhookSubscription { Id = 1, UserId = 10, Url = "https://hook.example/deal", EventTypes = "deal.completed", Active = true },
            new WebhookSubscription { Id = 2, UserId = 11, Url = "https://hook.example/other", EventTypes = "user.registered", Active = true },
        ]);

        var service = new WebhookService(_uow.Object);
        var sent = await service.DispatchEventAsync("deal.completed", "{\"id\":1}");

        Assert.Equal(1, sent);
        _outbox.Verify(r => r.Add(It.Is<OutboxMessage>(m =>
            m.EventType == "deal.completed" && m.Destination == "https://hook.example/deal")), Times.Once);
    }

    [Fact]
    public async Task Unsubscribe_NotOwner_ReturnsFalse()
    {
        _webhooks.Setup(r => r.GetAsync(1L)).ReturnsAsync(
            new WebhookSubscription { Id = 1, UserId = 10, Active = true });

        var service = new WebhookService(_uow.Object);
        var result = await service.UnsubscribeAsync(1, 999);
        Assert.False(result);
    }
}