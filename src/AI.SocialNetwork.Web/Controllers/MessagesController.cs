using AI.SocialNetwork.Application.Contracts;
using AI.SocialNetwork.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AI.SocialNetwork.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly IMessageService _messageService;

    public MessagesController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpPost("conversations")]
    public async Task<ActionResult<Conversation>> CreateConversation(CreateConversationRequest request)
    {
        var conversation = await _messageService.CreateConversationAsync(request.UserIdA, request.UserIdB);
        return conversation == null ? BadRequest() : Ok(conversation);
    }

    [HttpGet("conversations/{conversationId:long}")]
    public async Task<ActionResult<IEnumerable<Message>>> GetConversation(long conversationId)
    {
        return Ok(await _messageService.GetConversationAsync(conversationId));
    }

    [HttpPost("conversations/{conversationId:long}/messages")]
    public async Task<ActionResult<Message>> Send(long conversationId, SendMessageRequest request)
    {
        return Ok(await _messageService.SendAsync(conversationId, request.SenderId, request.Body));
    }
}

public record CreateConversationRequest(long UserIdA, long UserIdB);
public record SendMessageRequest(long SenderId, string Body);