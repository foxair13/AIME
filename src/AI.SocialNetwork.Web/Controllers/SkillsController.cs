using AI.SocialNetwork.Application.Contracts;
using AI.SocialNetwork.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AI.SocialNetwork.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SkillsController : ControllerBase
{
    private readonly ISkillService _skillService;

    public SkillsController(ISkillService skillService)
    {
        _skillService = skillService;
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<Skill>> GetById(long id)
    {
        var skill = await _skillService.GetSkillAsync(id);
        return skill == null ? NotFound() : Ok(skill);
    }

    [HttpGet("categories")]
    public async Task<ActionResult<IEnumerable<SkillCategory>>> GetCategories()
    {
        return Ok(await _skillService.GetCategoriesAsync());
    }

    [HttpGet("by-category/{categoryId:long}")]
    public async Task<ActionResult<IEnumerable<Skill>>> GetByCategory(long categoryId)
    {
        return Ok(await _skillService.GetByCategoryAsync(categoryId));
    }

    [HttpPost]
    public async Task<ActionResult<Skill>> Create(CreateSkillRequest request)
    {
        var skill = await _skillService.AddSkillAsync(request.Name, request.ParentId, request.CategoryId, request.Description);
        return skill == null ? BadRequest() : CreatedAtAction(nameof(GetById), new { id = skill.Id }, skill);
    }

    [HttpPost("user-skills")]
    public async Task<ActionResult<UserSkill>> AddUserSkill(AddUserSkillRequest request)
    {
        return Ok(await _skillService.AddUserSkillAsync(request.UserId, request.SkillId, request.Level));
    }

    [HttpGet("user/{userId:long}")]
    public async Task<ActionResult<IEnumerable<UserSkill>>> GetUserSkills(long userId)
    {
        return Ok(await _skillService.GetUserSkillsAsync(userId));
    }
}

public record CreateSkillRequest(string Name, long? ParentId, long CategoryId, string? Description);
public record AddUserSkillRequest(long UserId, long SkillId, int Level);