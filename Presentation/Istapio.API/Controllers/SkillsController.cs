using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Istapio.API.Controllers.Common;
using Istapio.Application.Models.DTOs.Skill;
using Istapio.Application.Services.Internal.Interfaces;
using Istapio.Domain.Constants;

namespace Istapio.API.Controllers;

/// <summary>
/// Controller for managing skills
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class SkillsController : BaseController
{
    private readonly ISkillService _skillService;

    /// <summary>
    /// Initializes a new instance of the SkillsController
    /// </summary>
    /// <param name="skillService">The skill service instance</param>
    public SkillsController(ISkillService skillService)
    {
        _skillService = skillService;
    }

    /// <summary>
    /// Retrieves a skill by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the skill</param>
    /// <returns>The requested skill</returns>
    /// <response code="200">Returns the skill</response>
    /// <response code="404">If the skill is not found</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GetSkillDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var skill = await _skillService.GetByIdAsync(id);
        return Success(skill);
    }

    /// <summary>
    /// Retrieves all skills
    /// </summary>
    /// <returns>A list of all skills</returns>
    /// <response code="200">Returns the list of skills</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<GetSkillDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var skills = await _skillService.GetAllAsync();
        return Success(skills);
    }

    /// <summary>
    /// Retrieves a paginated list of skills
    /// </summary>
    /// <param name="pageIndex">The page number (default: 1)</param>
    /// <param name="pageSize">The number of items per page (default: 10)</param>
    /// <returns>A paginated list of skills with total count</returns>
    /// <response code="200">Returns the paginated skills</response>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
    {
        var (items, totalCount) = await _skillService.GetPagedAsync(pageIndex, pageSize);
        return Success(new { items, totalCount, pageIndex, pageSize });
    }

    /// <summary>
    /// Creates a new skill
    /// </summary>
    /// <param name="dto">The skill data to create</param>
    /// <returns>The newly created skill</returns>
    /// <response code="201">Returns the newly created skill</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user does not have the required role</response>
    [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Admin},{Roles.Moderator}")]
    [HttpPost]
    [ProducesResponseType(typeof(GetSkillDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateSkillDto dto)
    {
        var skill = await _skillService.CreateAsync(dto);
        return Created(skill, "Skill created successfully");
    }

    /// <summary>
    /// Updates an existing skill by its identifier
    /// </summary>
    /// <param name="id">The unique identifier of the skill</param>
    /// <param name="dto">The updated skill data</param>
    /// <returns>The updated skill</returns>
    /// <response code="200">Returns the updated skill</response>
    /// <response code="400">If the ID in route doesn't match the ID in body</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user does not have the required role</response>
    /// <response code="404">If the skill is not found</response>
    [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Admin},{Roles.Moderator}")]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(GetSkillDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSkillDto dto)
    {
        if (id != dto.Id)
            return BadRequest("ID mismatch");

        var skill = await _skillService.UpdateAsync(dto);
        return Success(skill, "Skill updated successfully");
    }

    /// <summary>
    /// Soft deletes a skill by its identifier
    /// </summary>
    /// <param name="id">The unique identifier of the skill to delete</param>
    /// <returns>No content</returns>
    /// <response code="204">If the skill was successfully deleted</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user does not have the required role</response>
    /// <response code="404">If the skill is not found</response>
    [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Admin},{Roles.Moderator}")]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _skillService.DeleteAsync(id);
        return NoContent("Skill deleted successfully");
    }
}