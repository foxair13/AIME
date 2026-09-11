using AI.SocialNetwork.Application.Contracts;
using AI.SocialNetwork.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AI.SocialNetwork.Web.Controllers;

// A11: webhook-подписки — события доставляются через Transactional Outbox
[ApiController]
[Route("api/webhooks")]
[Authorize]
public class WebhooksController : ControllerBase
{
    private readonly IWebhookService _webhookService;

    public WebhooksController(IWebhookService webhookService)
    {
        _webhookService = webhookService;
    }

    [HttpGet("{userId:long}")]
    public async Task<ActionResult<IEnumerable<WebhookSubscription>>> GetMine(long userId)
    {
        return Ok(await _webhookService.GetByUserIdAsync(userId));
    }

    [HttpPost]
    public async Task<ActionResult<WebhookSubscription>> Subscribe(SubscribeWebhookRequest request)
    {
        var sub = await _webhookService.SubscribeAsync(request.UserId, request.Url, request.EventTypes, request.Secret);
        return sub == null ? BadRequest() : Ok(sub);
    }

    [HttpDelete("{subscriptionId:long}/{userId:long}")]
    public async Task<IActionResult> Unsubscribe(long subscriptionId, long userId)
    {
        return await _webhookService.UnsubscribeAsync(subscriptionId, userId) ? NoContent() : NotFound();
    }

    [HttpPost("dispatch")]
    [Authorize(Roles = "admin,moderator")]
    public async Task<ActionResult<int>> Dispatch(DispatchWebhookEventRequest request)
    {
        var sent = await _webhookService.DispatchEventAsync(request.EventType, request.PayloadJson);
        return Ok(sent);
    }
}

public record SubscribeWebhookRequest(long UserId, string Url, string EventTypes, string? Secret);
public record DispatchWebhookEventRequest(string EventType, string? PayloadJson);