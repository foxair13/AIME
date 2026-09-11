using AI.SocialNetwork.Application.Contracts;
using AI.SocialNetwork.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AI.SocialNetwork.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompetenceController : ControllerBase
{
    private readonly ICompetenceService _competenceService;

    public CompetenceController(ICompetenceService competenceService)
    {
        _competenceService = competenceService;
    }

    [HttpGet("roles")]
    public async Task<ActionResult<IEnumerable<ProfessionalRole>>> GetRoles()
    {
        return Ok(await _competenceService.GetRolesAsync());
    }

    [HttpGet("role-fit")]
    public async Task<ActionResult<decimal>> GetRoleFit([FromQuery] long userId, [FromQuery] long roleId)
    {
        return Ok(await _competenceService.GetRoleFitAsync(userId, roleId));
    }

    [HttpGet("current-level")]
    public async Task<ActionResult<decimal>> GetCurrentLevel([FromQuery] long userId, [FromQuery] long skillId)
    {
        return Ok(await _competenceService.GetCurrentLevelAsync(userId, skillId));
    }
}