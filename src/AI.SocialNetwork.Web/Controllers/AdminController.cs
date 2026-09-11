using AI.SocialNetwork.Application.Contracts;
using AI.SocialNetwork.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AI.SocialNetwork.Web.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = UserRoles.Admin)]
public class AdminController : ControllerBase
{
    private readonly IUserService _userService;

    public AdminController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("users/{id:long}/role")]
    public async Task<IActionResult> SetRole(long id, SetRoleRequest request)
    {
        var user = await _userService.SetRoleAsync(id, request.Role);
        return user == null ? NotFound() : Ok(user);
    }

    [HttpPost("users/{id:long}/deactivate")]
    public async Task<IActionResult> Deactivate(long id)
    {
        var user = await _userService.SetActiveAsync(id, false);
        return user == null ? NotFound() : Ok(user);
    }

    [HttpGet("whoami")]
    public IActionResult WhoAmI()
    {
        return Ok(new
        {
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            Name = User.FindFirstValue(ClaimTypes.Name),
            Role = User.FindFirstValue(ClaimTypes.Role)
        });
    }
}

public record SetRoleRequest(string Role);