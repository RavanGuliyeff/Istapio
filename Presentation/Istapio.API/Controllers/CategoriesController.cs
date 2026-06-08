using Microsoft.AspNetCore.Mvc;
using Istapio.API.Controllers.Common;
using Istapio.Application.Models.DTOs.Category;
using Istapio.Application.Services.Internal.Interfaces;

namespace Istapio.API.Controllers;

/// <summary>
/// Controller for managing product categories
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class CategoriesController : BaseController
{
    private readonly ICategoryService _categoryService;

    /// <summary>
    /// Initializes a new instance of the CategoriesController
    /// </summary>
    /// <param name="categoryService">The category service instance</param>
    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// <summary>
    /// Retrieves a category by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the category</param>
    /// <returns>The requested category</returns>
    /// <response code="200">Returns the category</response>
    /// <response code="404">If the category is not found</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GetCategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        return Success(category);
    }

    /// <summary>
    /// Retrieves all categories
    /// </summary>
    /// <returns>A list of all categories</returns>
    /// <response code="200">Returns the list of categories</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<GetCategoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _categoryService.GetAllAsync();
        return Success(categories);
    }

    /// <summary>
    /// Retrieves a paginated list of categories
    /// </summary>
    /// <param name="pageIndex">The page number (default: 1)</param>
    /// <param name="pageSize">The number of items per page (default: 10)</param>
    /// <returns>A paginated list of categories with total count</returns>
    /// <response code="200">Returns the paginated categories</response>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
    {
        var (items, totalCount) = await _categoryService.GetPagedAsync(pageIndex, pageSize);
        return Success(new { items, totalCount, pageIndex, pageSize });
    }

    /// <summary>
    /// Creates a new category
    /// </summary>
    /// <param name="dto">The category data to create</param>
    /// <returns>The newly created category</returns>
    /// <response code="201">Returns the newly created category</response>
    /// <response code="400">If the request data is invalid</response>
    [HttpPost]
    [ProducesResponseType(typeof(GetCategoryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
    {
        var category = await _categoryService.CreateAsync(dto);
        return Created(category, "Category created successfully");
    }

    /// <summary>
    /// Updates an existing category by its identifier
    /// </summary>
    /// <param name="id">The unique identifier of the category</param>
    /// <param name="dto">The updated category data</param>
    /// <returns>The updated category</returns>
    /// <response code="200">Returns the updated category</response>
    /// <response code="400">If the ID in route doesn't match the ID in body</response>
    /// <response code="404">If the category is not found</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(GetCategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryDto dto)
    {
        if (id != dto.Id)
            return BadRequest("ID mismatch");

        var category = await _categoryService.UpdateAsync(dto);
        return Success(category, "Category updated successfully");
    }

    /// <summary>
    /// Soft deletes a category by its identifier
    /// </summary>
    /// <param name="id">The unique identifier of the category to delete</param>
    /// <returns>No content</returns>
    /// <response code="204">If the category was successfully deleted</response>
    /// <response code="404">If the category is not found</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _categoryService.DeleteAsync(id);
        return NoContent("Category deleted successfully");
    }
}