using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Istapio.API.Controllers.Common;
using Istapio.Application.Models.DTOs.VacationType;
using Istapio.Application.Services.Internal.Interfaces;
using Istapio.Domain.Constants;

namespace Istapio.API.Controllers;

/// <summary>
/// Controller for managing vacation types
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class VacationTypesController : BaseController
{
    private readonly IVacationTypeService _vacationTypeService;

    /// <summary>
    /// Initializes a new instance of the VacationTypesController
    /// </summary>
    /// <param name="vacationTypeService">The vacation type service instance</param>
    public VacationTypesController(IVacationTypeService vacationTypeService)
    {
        _vacationTypeService = vacationTypeService;
    }

    /// <summary>
    /// Retrieves a vacation type by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the vacation type</param>
    /// <returns>The requested vacation type</returns>
    /// <response code="200">Returns the vacation type</response>
    /// <response code="404">If the vacation type is not found</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GetVacationTypeDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var vacationType = await _vacationTypeService.GetByIdAsync(id);
        return Success(vacationType);
    }

    /// <summary>
    /// Retrieves all vacation types
    /// </summary>
    /// <returns>A list of all vacation types</returns>
    /// <response code="200">Returns the list of vacation types</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<GetVacationTypeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var vacationTypes = await _vacationTypeService.GetAllAsync();
        return Success(vacationTypes);
    }

    /// <summary>
    /// Retrieves a paginated list of vacation types
    /// </summary>
    /// <param name="pageIndex">The page number (default: 1)</param>
    /// <param name="pageSize">The number of items per page (default: 10)</param>
    /// <returns>A paginated list of vacation types with total count</returns>
    /// <response code="200">Returns the paginated vacation types</response>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
    {
        var (items, totalCount) = await _vacationTypeService.GetPagedAsync(pageIndex, pageSize);
        return Success(new { items, totalCount, pageIndex, pageSize });
    }

    /// <summary>
    /// Creates a new vacation type
    /// </summary>
    /// <param name="dto">The vacation type data to create</param>
    /// <returns>The newly created vacation type</returns>
    /// <response code="201">Returns the newly created vacation type</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user does not have the required role</response>
    [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Admin},{Roles.Moderator}")]
    [HttpPost]
    [ProducesResponseType(typeof(GetVacationTypeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateVacationTypeDto dto)
    {
        var vacationType = await _vacationTypeService.CreateAsync(dto);
        return Created(vacationType, "Vacation type created successfully");
    }

    /// <summary>
    /// Updates an existing vacation type by its identifier
    /// </summary>
    /// <param name="id">The unique identifier of the vacation type</param>
    /// <param name="dto">The updated vacation type data</param>
    /// <returns>The updated vacation type</returns>
    /// <response code="200">Returns the updated vacation type</response>
    /// <response code="400">If the ID in route doesn't match the ID in body</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user does not have the required role</response>
    /// <response code="404">If the vacation type is not found</response>
    [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Admin},{Roles.Moderator}")]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(GetVacationTypeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateVacationTypeDto dto)
    {
        if (id != dto.Id)
            return BadRequest("ID mismatch");

        var vacationType = await _vacationTypeService.UpdateAsync(dto);
        return Success(vacationType, "Vacation type updated successfully");
    }

    /// <summary>
    /// Soft deletes a vacation type by its identifier
    /// </summary>
    /// <param name="id">The unique identifier of the vacation type to delete</param>
    /// <returns>No content</returns>
    /// <response code="204">If the vacation type was successfully deleted</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="403">If the user does not have the required role</response>
    /// <response code="404">If the vacation type is not found</response>
    [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Admin},{Roles.Moderator}")]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _vacationTypeService.DeleteAsync(id);
        return NoContent("Vacation type deleted successfully");
    }
}