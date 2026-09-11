using AI.SocialNetwork.Application.Contracts;
using AI.SocialNetwork.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AI.SocialNetwork.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DealsController : ControllerBase
{
    private readonly IDealService _dealService;

    public DealsController(IDealService dealService)
    {
        _dealService = dealService;
    }

    [HttpPost]
    public async Task<ActionResult<Deal>> Create(CreateDealRequest request)
    {
        var deal = await _dealService.CreateAsync(request.ClientId, request.ContractorId, request.Amount, request.ContractTerms);
        return deal == null ? BadRequest() : Ok(deal);
    }

    [HttpPost("{dealId:long}/escrow")]
    public async Task<ActionResult<Deal>> Escrow(long dealId, EscrowRequest request)
    {
        var deal = await _dealService.EscrowAsync(dealId, request.Amount);
        return deal == null ? NotFound() : Ok(deal);
    }

    [HttpPost("{dealId:long}/milestones")]
    public async Task<ActionResult<Deal>> AddMilestone(long dealId, MilestoneRequest request)
    {
        var deal = await _dealService.AddMilestoneAsync(dealId, request.Title, request.Amount);
        return deal == null ? NotFound() : Ok(deal);
    }

    [HttpPost("milestones/{milestoneId:long}/complete")]
    public async Task<ActionResult<Deal>> CompleteMilestone(long milestoneId)
    {
        var deal = await _dealService.CompleteMilestoneAsync(milestoneId);
        return deal == null ? NotFound() : Ok(deal);
    }

    [HttpPost("{dealId:long}/complete")]
    public async Task<ActionResult<Deal>> Complete(long dealId)
    {
        var deal = await _dealService.CompleteAsync(dealId);
        return deal == null ? NotFound() : Ok(deal);
    }

    [HttpPost("{dealId:long}/dispute")]
    public async Task<ActionResult<Deal>> Dispute(long dealId)
    {
        var deal = await _dealService.DisputeAsync(dealId);
        return deal == null ? NotFound() : Ok(deal);
    }
}

public record CreateDealRequest(long ClientId, long ContractorId, decimal Amount, string? ContractTerms);
public record EscrowRequest(decimal Amount);
public record MilestoneRequest(string Title, decimal Amount);