using AI.SocialNetwork.Application.Contracts;
using AI.SocialNetwork.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AI.SocialNetwork.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AgentsController : ControllerBase
{
    private readonly IAgentService _agentService;

    public AgentsController(IAgentService agentService)
    {
        _agentService = agentService;
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<Agent>> GetById(long id)
    {
        var agent = await _agentService.GetByIdAsync(id);
        return agent == null ? NotFound() : Ok(agent);
    }

    [HttpGet("by-user/{userId:long}")]
    public async Task<ActionResult<IEnumerable<Agent>>> GetByUserId(long userId)
    {
        return Ok(await _agentService.GetByUserIdAsync(userId));
    }

    [HttpPost]
    public async Task<ActionResult<Agent>> Create(CreateAgentRequest request)
    {
        var agent = await _agentService.CreateAsync(request.OwnerId, request.Name, request.SystemPrompt, request.CommunicationStyle);
        return agent == null ? BadRequest() : CreatedAtAction(nameof(GetById), new { id = agent.Id }, agent);
    }

    [HttpPut("{agentId:long}")]
    public async Task<ActionResult<Agent>> Update(long agentId, UpdateAgentRequest request)
    {
        var agent = await _agentService.UpdateAsync(agentId, request.Name, request.SystemPrompt, request.CommunicationStyle, request.IsActive, request.LLMModel);
        return agent == null ? NotFound() : Ok(agent);
    }

    [HttpPost("{agentId:long}/toggle/{isActive:bool}")]
    public async Task<ActionResult<Agent>> ToggleActive(long agentId, bool isActive)
    {
        var agent = await _agentService.ToggleActiveAsync(agentId, isActive);
        return agent == null ? NotFound() : Ok(agent);
    }

    [HttpPost("{agentId:long}/messages")]
    public async Task<ActionResult<AgentMessage>> SendMessage(long agentId, SendAgentMessageRequest request)
    {
        return Ok(await _agentService.SendMessageAsync(agentId, request.TargetAgentId, request.Content));
    }

    // --- A13: права, лимиты, журнал решений (explainability) ---

    [HttpGet("permissions/{ownerId:long}")]
    public async Task<ActionResult<IEnumerable<AgentPermission>>> GetPermissions(long ownerId)
    {
        return Ok(await _agentService.GetPermissionsByOwnerAsync(ownerId));
    }

    [HttpPost("permissions/{ownerId:long}")]
    public async Task<ActionResult<AgentPermission>> SetPermission(long ownerId, SetPermissionRequest request)
    {
        var permission = await _agentService.SetPermissionAsync(ownerId, request.Action, request.Allowed, request.DailyLimitAmount, request.DailyLimitCount);
        return permission == null ? BadRequest() : Ok(permission);
    }

    [HttpGet("{agentId:long}/logs")]
    public async Task<ActionResult<IEnumerable<AgentActionLog>>> GetLogs(long agentId, [FromQuery] int limit = 50)
    {
        return Ok(await _agentService.GetActionLogsAsync(agentId, limit));
    }

    [HttpPost("{agentId:long}/can-execute")]
    public async Task<ActionResult<PermissionResult>> CanExecute(long agentId, CanExecuteRequest request)
    {
        // Проверка «разрешено ли агенту действие» без его выполнения (A13)
        return Ok(await _agentService.CanExecuteAsync(agentId, request.Action, request.Amount, request.Reason, request.InputJson));
    }
}

public record CreateAgentRequest(long OwnerId, string Name, string? SystemPrompt, string? CommunicationStyle);
public record SendAgentMessageRequest(long? TargetAgentId, string Content);
public record UpdateAgentRequest(string? Name, string? SystemPrompt, string? CommunicationStyle, bool? IsActive, string? LLMModel);
public record SetPermissionRequest(string Action, bool Allowed, decimal? DailyLimitAmount, int? DailyLimitCount);
public record CanExecuteRequest(string Action, decimal Amount = 0, string? Reason = null, string? InputJson = null);