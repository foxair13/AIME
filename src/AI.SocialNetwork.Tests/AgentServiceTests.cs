using AI.SocialNetwork.Application.Services;
using AI.SocialNetwork.Domain.Entities;
using AI.SocialNetwork.Infrastructure.Repositories.Contracts;
using AI.SocialNetwork.Infrastructure.UnitOfWork.Contracts;
using Moq;

namespace AI.SocialNetwork.Tests;

public class AgentServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IGenericRepository<Agent>> _agents = new();
    private readonly Mock<IGenericRepository<AgentPermission>> _permissions = new();
    private readonly Mock<IGenericRepository<AgentActionLog>> _logs = new();
    private readonly Mock<IGenericRepository<AgentMessage>> _messages = new();
    private readonly Mock<IGenericRepository<OutboxMessage>> _outbox = new();

    public AgentServiceTests()
    {
        _uow.Setup(u => u.Agents).Returns(_agents.Object);
        _uow.Setup(u => u.AgentPermissions).Returns(_permissions.Object);
        _uow.Setup(u => u.AgentActionLogs).Returns(_logs.Object);
        _uow.Setup(u => u.AgentMessages).Returns(_messages.Object);
        _uow.Setup(u => u.OutboxMessages).Returns(_outbox.Object);
    }

    private AgentService CreateService()
    {
        return new AgentService(_uow.Object);
    }

    private static Mock<IGenericRepository<T>> Repo<T>(IEnumerable<T> items) where T : class
    {
        var mock = new Mock<IGenericRepository<T>>();
        mock.Setup(r => r.GetAllAsync()).ReturnsAsync(items);
        return mock;
    }

    [Fact]
    public async Task CanExecute_NoPermissionForFinancialAction_IsDeniedByDefault()
    {
        var agent = new Agent { Id = 1, UserId = 10, IsActive = true };
        _agents.Setup(r => r.GetAsync(1L)).ReturnsAsync(agent);
        _permissions.Setup(r => r.GetAllAsync()).ReturnsAsync(Array.Empty<AgentPermission>());
        _logs.Setup(r => r.GetAllAsync()).ReturnsAsync(Array.Empty<AgentActionLog>());

        var service = CreateService();
        var denied = await service.CanExecuteAsync(1, "pay", 100m, null, null);
        Assert.False(denied.Allowed);

        var allowed = await service.CanExecuteAsync(1, "post", 0m, null, null);
        Assert.True(allowed.Allowed);
    }

    [Fact]
    public async Task CanExecute_DisabledAgent_IsDenied()
    {
        var agent = new Agent { Id = 1, UserId = 10, IsActive = false };
        _agents.Setup(r => r.GetAsync(1L)).ReturnsAsync(agent);

        var service = CreateService();
        var result = await service.CanExecuteAsync(1, "send_message", 0m, null, null);
        Assert.False(result.Allowed);
    }

    [Fact]
    public async Task CanExecute_ExplicitDeny_IsDenied()
    {
        var agent = new Agent { Id = 1, UserId = 10, IsActive = true };
        _agents.Setup(r => r.GetAsync(1L)).ReturnsAsync(agent);
        _permissions.Setup(r => r.GetAllAsync()).ReturnsAsync([
            new AgentPermission { Id = 1, OwnerId = 10, Action = "create_deal", Allowed = false }
        ]);

        var service = CreateService();
        var result = await service.CanExecuteAsync(1, "create_deal", 0m, null, null);
        Assert.False(result.Allowed);
    }

    [Fact]
    public async Task CanExecute_DailyCountLimit_IsEnforced()
    {
        var agent = new Agent { Id = 1, UserId = 10, IsActive = true };
        _agents.Setup(r => r.GetAsync(1L)).ReturnsAsync(agent);
        _permissions.Setup(r => r.GetAllAsync()).ReturnsAsync([
            new AgentPermission { Id = 1, OwnerId = 10, Action = "send_message", Allowed = true, DailyLimitCount = 2 }
        ]);
        var today = DateTime.UtcNow.Date;
        _logs.Setup(r => r.GetAllAsync()).ReturnsAsync([
            new AgentActionLog { Id = 1, AgentId = 1, OwnerId = 10, Action = "send_message", Allowed = true, CreatedAt = today.AddHours(1) },
            new AgentActionLog { Id = 2, AgentId = 1, OwnerId = 10, Action = "send_message", Allowed = true, CreatedAt = today.AddHours(2) },
        ]);

        var service = CreateService();
        var result = await service.CanExecuteAsync(1, "send_message", 0m, null, null);
        Assert.False(result.Allowed);
        Assert.Contains("лимит", result.Reason, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CanExecute_WithinDailyCountLimit_IsAllowed()
    {
        var agent = new Agent { Id = 1, UserId = 10, IsActive = true };
        _agents.Setup(r => r.GetAsync(1L)).ReturnsAsync(agent);
        _permissions.Setup(r => r.GetAllAsync()).ReturnsAsync([
            new AgentPermission { Id = 1, OwnerId = 10, Action = "send_message", Allowed = true, DailyLimitCount = 5 }
        ]);
        _logs.Setup(r => r.GetAllAsync()).ReturnsAsync(Array.Empty<AgentActionLog>());

        var service = CreateService();
        var result = await service.CanExecuteAsync(1, "send_message", 0m, null, null);
        Assert.True(result.Allowed);
    }

    [Fact]
    public async Task SendMessage_ByDefaultAllowed_Sends()
    {
        var agent = new Agent { Id = 1, UserId = 10, Name = "Тест", IsActive = true };
        _agents.Setup(r => r.GetAsync(1L)).ReturnsAsync(agent);
        _permissions.Setup(r => r.GetAllAsync()).ReturnsAsync([]);
        _logs.Setup(r => r.GetAllAsync()).ReturnsAsync(Array.Empty<AgentActionLog>());

        var service = CreateService();
        var msg = await service.SendMessageAsync(1, null, "привет");

        Assert.NotNull(msg);
        // Без Ollama сервис возвращает fallback-ответ (не пустой и от имени агента)
        Assert.Contains("Тест", msg.Content);
        _messages.Verify(r => r.Add(It.Is<AgentMessage>(m => !string.IsNullOrWhiteSpace(m.Content))), Times.Once);
    }

    [Fact]
    public async Task SendMessage_ExplicitDeny_Throws()
    {
        var agent = new Agent { Id = 1, UserId = 10, IsActive = true };
        _agents.Setup(r => r.GetAsync(1L)).ReturnsAsync(agent);
        _permissions.Setup(r => r.GetAllAsync()).ReturnsAsync([
            new AgentPermission { Id = 1, OwnerId = 10, Action = "send_message", Allowed = false }
        ]);
        _logs.Setup(r => r.GetAllAsync()).ReturnsAsync(Array.Empty<AgentActionLog>());

        var service = CreateService();
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.SendMessageAsync(1, null, "привет"));
    }

    [Fact]
    public async Task SetPermission_IsPersisted()
    {
        _permissions.Setup(r => r.GetAllAsync()).ReturnsAsync(Array.Empty<AgentPermission>());
        AgentPermission? saved = null;
        _permissions.Setup(r => r.Add(It.IsAny<AgentPermission>())).Callback<AgentPermission>(p => saved = p);

        var service = CreateService();
        var result = await service.SetPermissionAsync(10, "create_deal", true, 1000m, 3);

        Assert.NotNull(result);
        Assert.Equal(10, result!.OwnerId);
        Assert.True(result.Allowed);
        Assert.Equal(1000m, result.DailyLimitAmount);
        Assert.Equal(3, result.DailyLimitCount);
        Assert.NotNull(saved);
    }
}