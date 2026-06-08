using Microsoft.AspNetCore.Mvc;
using Istapio.API.Controllers.Common;
using Istapio.Application.Models.DTOs.Company;
using Istapio.Application.Services.Internal.Interfaces;

namespace Istapio.API.Controllers;

/// <summary>
/// Controller for managing companies
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class CompaniesController : BaseController
{
    private readonly ICompanyService _companyService;

    /// <summary>
    /// Initializes a new instance of the CompaniesController
    /// </summary>
    /// <param name="companyService">The company service instance</param>
    public CompaniesController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    /// <summary>
    /// Retrieves a company by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the company</param>
    /// <returns>The requested company</returns>
    /// <response code="200">Returns the company</response>
    /// <response code="404">If the company is not found</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GetCompanyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var company = await _companyService.GetByIdAsync(id);
        return Success(company);
    }

    /// <summary>
    /// Retrieves all companies
    /// </summary>
    /// <returns>A list of all companies</returns>
    /// <response code="200">Returns the list of companies</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<GetCompanyDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var companies = await _companyService.GetAllAsync();
        return Success(companies);
    }

    /// <summary>
    /// Retrieves a paginated list of companies
    /// </summary>
    /// <param name="pageIndex">The page number (default: 1)</param>
    /// <param name="pageSize">The number of items per page (default: 10)</param>
    /// <returns>A paginated list of companies with total count</returns>
    /// <response code="200">Returns the paginated companies</response>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
    {
        var (items, totalCount) = await _companyService.GetPagedAsync(pageIndex, pageSize);
        return Success(new { items, totalCount, pageIndex, pageSize });
    }

    /// <summary>
    /// Creates a new company
    /// </summary>
    /// <param name="dto">The company data to create</param>
    /// <returns>The newly created company</returns>
    /// <response code="201">Returns the newly created company</response>
    /// <response code="400">If the request data is invalid</response>
    [HttpPost]
    [ProducesResponseType(typeof(GetCompanyDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCompanyDto dto)
    {
        var company = await _companyService.CreateAsync(dto);
        return Created(company, "Company created successfully");
    }

    /// <summary>
    /// Updates an existing company by its identifier
    /// </summary>
    /// <param name="id">The unique identifier of the company</param>
    /// <param name="dto">The updated company data</param>
    /// <returns>The updated company</returns>
    /// <response code="200">Returns the updated company</response>
    /// <response code="400">If the ID in route doesn't match the ID in body</response>
    /// <response code="404">If the company is not found</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(GetCompanyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCompanyDto dto)
    {
        if (id != dto.Id)
            return BadRequest("ID mismatch");

        var company = await _companyService.UpdateAsync(dto);
        return Success(company, "Company updated successfully");
    }

    /// <summary>
    /// Soft deletes a company by its identifier
    /// </summary>
    /// <param name="id">The unique identifier of the company to delete</param>
    /// <returns>No content</returns>
    /// <response code="204">If the company was successfully deleted</response>
    /// <response code="404">If the company is not found</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _companyService.DeleteAsync(id);
        return NoContent("Company deleted successfully");
    }
}