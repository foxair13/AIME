using AI.SocialNetwork.Application.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace AI.SocialNetwork.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;

    public SearchController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SearchHit>>> Search(
        [FromQuery] string q,
        [FromQuery] string type = "users",
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20)
    {
        var results = await _searchService.SearchAsync(q, type, skip, take);
        return Ok(results);
    }

    [HttpGet("trending")]
    public async Task<ActionResult<IEnumerable<SearchHit>>> Trending([FromQuery] int take = 10)
    {
        return Ok(await _searchService.TrendingHashtagsAsync(take));
    }

    [HttpPost("hashtags")]
    public async Task<ActionResult> EnsureHashtag(EnsureHashtagRequest request)
    {
        var tag = await _searchService.GetOrAddHashtagAsync(request.Tag);
        return tag == null ? BadRequest() : Ok(tag);
    }
}

public record EnsureHashtagRequest(string Tag);