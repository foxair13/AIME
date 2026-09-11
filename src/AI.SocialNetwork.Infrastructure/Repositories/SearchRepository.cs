using AI.SocialNetwork.Infrastructure.DataContext;
using AI.SocialNetwork.Infrastructure.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace AI.SocialNetwork.Infrastructure.Repositories;

public class SearchRepository : ISearchRepository
{
    private readonly AISocialNetworkContext _dbContext;

    public SearchRepository(AISocialNetworkContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<SearchCandidate>> SearchAsync(string query, string type, int skip, int take)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return new List<SearchCandidate>();
        }

        FormattableString sql = type switch
        {
            "users" => BuildSql(
                "users",
                "\"Name\" AS Title, COALESCE(\"City\", \"Email\") AS Subtitle",
                "\"Name\"",
                query, skip, take),
            "skills" => BuildSql(
                "skills",
                "\"Name\" AS Title, \"Description\" AS Subtitle",
                "\"Name\"",
                query, skip, take),
            "posts" => BuildSql(
                "posts",
                "CASE WHEN length(\"Body\") > 80 THEN left(\"Body\", 80) || '...' ELSE \"Body\" END AS Title, \"AuthorId\"::text AS Subtitle",
                "\"Body\"",
                query, skip, take, true),
            "groups" => BuildSql(
                "groups",
                "\"Name\" AS Title, null AS Subtitle",
                "\"Name\"",
                query, skip, take, true),
            "hashtags" => BuildHashtagSql(query, skip, take),
            _ => FormattableStringFactory.Create(
                "SELECT 0::bigint AS Id, '' AS Type, '' AS Title, null AS Subtitle, 0::double precision AS Score LIMIT 0")
        };

        return await _dbContext.Database.SqlQuery<SearchCandidate>(sql).ToListAsync();
    }

    private static FormattableString BuildSql(
        string table,
        string select,
        string rankColumn,
        string query,
        int skip,
        int take,
        bool scopedToPublic = false)
    {
        var publicFilter = scopedToPublic ? " AND \"IsPublic\" = TRUE" : string.Empty;
        return FormattableStringFactory.Create(
            $"SELECT \"Id\"::bigint AS Id, '{table}' AS Type, {select}, " +
            $"CAST(ts_rank(search_vector, plainto_tsquery('russian', {{0}})) AS double precision) AS Score " +
            $"FROM {table} " +
            $"WHERE search_vector @@ plainto_tsquery('russian', {{0}}){publicFilter} " +
            $"ORDER BY ts_rank(search_vector, plainto_tsquery('russian', {{0}})) DESC, {rankColumn} ASC " +
            $"LIMIT {{2}} OFFSET {{1}}",
            query, skip, take);
    }

    private static FormattableString BuildHashtagSql(string query, int skip, int take)
    {
        return FormattableStringFactory.Create(
            $"SELECT \"Id\"::bigint AS Id, 'hashtag' AS Type, \"Tag\" AS Title, \"UsageCount\"::text AS Subtitle, " +
            $"\"UsageCount\"::double precision AS Score " +
            $"FROM hashtags " +
            $"WHERE search_vector @@ plainto_tsquery('simple', {{0}}) " +
            $"ORDER BY \"UsageCount\" DESC " +
            $"LIMIT {{2}} OFFSET {{1}}",
            query, skip, take);
    }
}