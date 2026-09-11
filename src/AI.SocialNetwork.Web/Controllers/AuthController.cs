using AI.SocialNetwork.Application.Contracts;
using AI.SocialNetwork.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AI.SocialNetwork.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResult>> Register(RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request.Email, request.Name, request.Password);
        return result == null ? Conflict("Пользователь с таким email уже существует") : Ok(result);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResult>> Login(LoginRequest request)
    {
        var result = await _authService.LoginAsync(request.Email, request.Password);
        return result == null ? Unauthorized("Неверный email или пароль") : Ok(result);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<object>> Me()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(idClaim, out var id))
        {
            return Unauthorized();
        }
        var user = await _authService.GetByIdAsync(id);
        return user == null ? NotFound() : Ok(new
        {
            user.Id,
            user.Email,
            user.Name,
            user.Role,
            user.City,
            user.Country,
            user.Bio,
            user.IsActive,
            user.TrustLevel
        });
    }
}

public record RegisterRequest(string Email, string Name, string Password);
public record LoginRequest(string Email, string Password);