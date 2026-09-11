using AI.SocialNetwork.Application.Contracts;
using AI.SocialNetwork.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AI.SocialNetwork.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SocialController : ControllerBase
{
    private readonly ISocialService _socialService;

    public SocialController(ISocialService socialService)
    {
        _socialService = socialService;
    }

    [HttpPost("posts")]
    public async Task<ActionResult<Post>> CreatePost(CreatePostRequest request)
    {
        var post = await _socialService.CreatePostAsync(request.AuthorId, request.GroupId, request.Body, request.ContentType, request.IsPublic);
        return post == null ? BadRequest() : Ok(post);
    }

    [HttpGet("feed/{userId:long}")]
    public async Task<ActionResult<IEnumerable<Post>>> GetFeed(long userId, [FromQuery] int skip = 0, [FromQuery] int take = 20)
    {
        return Ok(await _socialService.GetFeedAsync(userId, skip, take));
    }

    [HttpPost("groups")]
    public async Task<ActionResult<Group>> CreateGroup(CreateGroupRequest request)
    {
        var group = await _socialService.CreateGroupAsync(request.OwnerId, request.Name, request.IsPublic);
        return group == null ? BadRequest() : Ok(group);
    }

    [HttpPost("groups/{groupId:long}/members")]
    public async Task<ActionResult<Membership>> JoinGroup(long groupId, JoinGroupRequest request)
    {
        var membership = await _socialService.JoinGroupAsync(groupId, request.UserId);
        return membership == null ? BadRequest() : Ok(membership);
    }

    [HttpPost("forums")]
    public async Task<ActionResult<Forum>> CreateForum(CreateForumRequest request)
    {
        var forum = await _socialService.CreateForumAsync(request.GroupId, request.Name);
        return forum == null ? BadRequest() : Ok(forum);
    }

    [HttpPost("topics")]
    public async Task<ActionResult<ForumTopic>> CreateTopic(CreateTopicRequest request)
    {
        var topic = await _socialService.CreateTopicAsync(request.CategoryId, request.AuthorId, request.Title);
        return topic == null ? BadRequest() : Ok(topic);
    }

    [HttpPost("subscriptions")]
    public async Task<ActionResult<Subscription>> Subscribe(SubscribeRequest request)
    {
        var subscription = await _socialService.SubscribeAsync(request.UserId, request.TargetUserId, request.TargetType);
        return subscription == null ? BadRequest() : Ok(subscription);
    }
}

public record CreatePostRequest(long AuthorId, long? GroupId, string Body, string ContentType, bool IsPublic);
public record CreateGroupRequest(long OwnerId, string Name, bool IsPublic);
public record JoinGroupRequest(long UserId);
public record CreateForumRequest(long GroupId, string Name);
public record CreateTopicRequest(long CategoryId, long AuthorId, string Title);
public record SubscribeRequest(long UserId, long TargetUserId, string TargetType);