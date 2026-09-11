using AI.SocialNetwork.Application.Contracts;
using AI.SocialNetwork.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AI.SocialNetwork.Web.Controllers;

// A5: реферальные ссылки и приглашения
[ApiController]
[Route("api/")]
[Authorize]
public class ReferralsController : ControllerBase
{
    private readonly IReferralService _referralService;

    public ReferralsController(IReferralService referralService)
    {
        _referralService = referralService;
    }

    [HttpGet("referrals")]
    public async Task<ActionResult<ReferralLink>> GetMine([FromQuery] long userId)
    {
        var link = await _referralService.GetOrCreateAsync(userId);
        return Ok(link);
    }

    [HttpPost("referrals/apply")]
    public async Task<ActionResult> Apply(ApplyReferralRequest request)
    {
        var user = await _referralService.ApplyCodeAsync(request.Code, request.UserId);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpGet("referrals/{userId:long}/invited")]
    public async Task<ActionResult<IEnumerable<User>>> GetInvited(long userId)
    {
        return Ok(await _referralService.GetReferralsAsync(userId));
    }
}

public record ApplyReferralRequest(string Code, long UserId);