using AI.SocialNetwork.Application.Contracts;
using AI.SocialNetwork.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AI.SocialNetwork.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<User>> GetById(long id)
    {
        var user = await _userService.GetByIdAsync(id);
        return user == null ? NotFound() : Ok(user);
    }

    [HttpGet("by-email/{email}")]
    public async Task<ActionResult<User>> GetByEmail(string email)
    {
        var user = await _userService.GetByEmailAsync(email);
        return user == null ? NotFound() : Ok(user);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetAll()
    {
        return Ok(await _userService.GetAllAsync());
    }

    [HttpPost]
    public async Task<ActionResult<User>> Create(CreateUserRequest request)
    {
        var user = await _userService.CreateAsync(request.Email, request.Name, request.City, request.Country);
        return user == null ? BadRequest() : CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<User>> Update(long id, UpdateUserRequest request)
    {
        var user = await _userService.UpdateAsync(id, request.Name, request.Bio, request.City, request.Country);
        return user == null ? NotFound() : Ok(user);
    }

    [HttpDelete("{id:long}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> Delete(long id)
    {
        return await _userService.DeleteAsync(id) ? NoContent() : NotFound();
    }
}

public record CreateUserRequest(string Email, string Name, string? City, string? Country);
public record UpdateUserRequest(string Name, string? Bio, string? City, string? Country);