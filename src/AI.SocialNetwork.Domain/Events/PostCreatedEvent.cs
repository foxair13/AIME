namespace AI.SocialNetwork.Domain.Events;

public record PostCreatedEvent(long PostId, long AuthorId, long? GroupId, string Body, bool IsPublic)
{
    public static PostCreatedEvent From(Entities.Post post)
        => new(post.Id, post.AuthorId, post.GroupId, post.Body ?? string.Empty, post.IsPublic);
}