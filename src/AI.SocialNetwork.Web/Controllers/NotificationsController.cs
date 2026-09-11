using AI.SocialNetwork.Application.Contracts;
using AI.SocialNetwork.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace AI.SocialNetwork.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly IHubContext<AI.SocialNetwork.Web.Hubs.NotificationsHub> _hub;

    public NotificationsController(INotificationService notificationService, IHubContext<AI.SocialNetwork.Web.Hubs.NotificationsHub> hub)
    {
        _notificationService = notificationService;
        _hub = hub;
    }

    [HttpGet("unread/{userId:long}")]
    public async Task<ActionResult<IEnumerable<Notification>>> GetUnread(long userId)
    {
        return Ok(await _notificationService.GetUnreadAsync(userId));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Notification>>> GetMine([FromQuery] long userId)
    {
        return Ok(await _notificationService.GetAllAsync(userId));
    }

    [HttpPost]
    public async Task<ActionResult<Notification>> Create(CreateNotificationRequest request)
    {
        var notification = await _notificationService.CreateAsync(request.UserId, request.Type, request.PayloadJson, request.Channel);
        if (notification == null)
        {
            return BadRequest();
        }
        // Реальная доставка через SignalR (событие из Outbox в группу пользователя)
        await _hub.Clients.Group($"user-{request.UserId}").SendAsync("notification", new
        {
            notification.Id,
            notification.UserId,
            notification.Type,
            notification.PayloadJson,
            notification.Channel,
            notification.CreatedAt
        });
        return Ok(notification);
    }

    [HttpPost("{notificationId:long}/read")]
    public async Task<IActionResult> MarkRead(long notificationId)
    {
        await _notificationService.MarkReadAsync(notificationId);
        return NoContent();
    }
}

public record CreateNotificationRequest(long UserId, string Type, string PayloadJson, string Channel);