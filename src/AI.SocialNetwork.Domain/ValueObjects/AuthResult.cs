namespace AI.SocialNetwork.Domain.ValueObjects;

public record AuthResult(string Token, long UserId, string Email, string Name, string Role);