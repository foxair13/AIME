using AI.SocialNetwork.Application.Services;
using AI.SocialNetwork.Domain.Entities;
using AI.SocialNetwork.Infrastructure.Repositories.Contracts;
using AI.SocialNetwork.Infrastructure.UnitOfWork.Contracts;
using Moq;

namespace AI.SocialNetwork.Tests;

public class DealServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IGenericRepository<Deal>> _deals = new();
    private readonly Mock<IGenericRepository<DealMilestone>> _milestones = new();
    private readonly Mock<IGenericRepository<OutboxMessage>> _outbox = new();

    public DealServiceTests()
    {
        _uow.Setup(u => u.Deals).Returns(_deals.Object);
        _uow.Setup(u => u.DealMilestones).Returns(_milestones.Object);
        _uow.Setup(u => u.OutboxMessages).Returns(_outbox.Object);
    }

    private DealService CreateService()
    {
        return new DealService(_uow.Object);
    }

    [Fact]
    public async Task CreateAsync_SetsStatusCreated()
    {
        Deal? saved = null;
        _deals.Setup(r => r.Add(It.IsAny<Deal>())).Callback<Deal>(d => saved = d);

        var service = CreateService();
        var result = await service.CreateAsync(1, 2, 500m, "ТЗ");

        Assert.NotNull(result);
        Assert.Equal("created", result!.Status);
        _deals.Verify(r => r.Add(It.IsAny<Deal>()), Times.Once);
        _uow.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task EscrowAsync_SetsEscrowedStatus_AndAmount()
    {
        var deal = new Deal { Id = 7, Status = "created" };
        _deals.Setup(r => r.GetAsync(7L)).ReturnsAsync(deal);

        var service = CreateService();
        var result = await service.EscrowAsync(7, 500m);

        Assert.NotNull(result);
        Assert.Equal("escrowed", result!.Status);
        Assert.Equal(500m, result.EscrowHeld);
    }

    [Fact]
    public async Task EscrowAsync_UnknownDeal_ReturnsNull()
    {
        _deals.Setup(r => r.GetAsync(999L)).ReturnsAsync((Deal?)null);

        var service = CreateService();
        var result = await service.EscrowAsync(999, 100m);

        Assert.Null(result);
    }

    [Fact]
    public async Task AddMilestoneAsync_AddsPendingMilestone()
    {
        var deal = new Deal { Id = 1 };
        _deals.Setup(r => r.GetAsync(1L)).ReturnsAsync(deal);

        var service = CreateService();
        var result = await service.AddMilestoneAsync(1, "Этап 1", 250m);

        Assert.NotNull(result);
        _milestones.Verify(r => r.Add(It.Is<DealMilestone>(m => m.Title == "Этап 1" && m.Amount == 250m && m.Status == "pending")), Times.Once);
    }

    [Fact]
    public async Task CompleteMilestoneAsync_ThrowsOnUnknownMilestone()
    {
        _milestones.Setup(r => r.GetAsync(5L)).ReturnsAsync((DealMilestone?)null);

        var service = CreateService();
        var result = await service.CompleteMilestoneAsync(5);

        Assert.Null(result);
    }

    [Fact]
    public async Task CompleteAsync_ReleasesEscrow()
    {
        var deal = new Deal { Id = 1, Status = "escrowed", EscrowHeld = 500m };
        _deals.Setup(r => r.GetAsync(1L)).ReturnsAsync(deal);

        var service = CreateService();
        var result = await service.CompleteAsync(1);

        Assert.NotNull(result);
        Assert.Equal("completed", result!.Status);
        Assert.Equal(0m, result.EscrowHeld);
    }

    [Fact]
    public async Task DisputeAsync_SetsDisputedStatus()
    {
        var deal = new Deal { Id = 1, Status = "escrowed" };
        _deals.Setup(r => r.GetAsync(1L)).ReturnsAsync(deal);

        var service = CreateService();
        var result = await service.DisputeAsync(1);

        Assert.NotNull(result);
        Assert.Equal("disputed", result!.Status);
    }
}