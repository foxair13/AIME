namespace AI.SocialNetwork.Infrastructure.Repositories.Contracts;

public sealed record SearchCandidate(long Id, string Type, string Title, string? Subtitle, double Score);

public interface ISearchRepository
{
    Task<IReadOnlyList<SearchCandidate>> SearchAsync(string query, string type, int skip, int take);
}