using Microsoft.AspNetCore.Mvc;
using Istapio.API.Controllers.Common;
using Istapio.Application.Models.DTOs.JobPost;
using Istapio.Application.Services.Internal.Interfaces;

namespace Istapio.API.Controllers;

/// <summary>
/// Controller for managing job postings
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class JobPostsController : BaseController
{
    private readonly IJobPostService _jobPostService;

    /// <summary>
    /// Initializes a new instance of the JobPostsController
    /// </summary>
    /// <param name="jobPostService">The job post service instance</param>
    public JobPostsController(IJobPostService jobPostService)
    {
        _jobPostService = jobPostService;
    }

    /// <summary>
    /// Retrieves a job post by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the job post</param>
    /// <returns>The requested job post</returns>
    /// <response code="200">Returns the job post</response>
    /// <response code="404">If the job post is not found</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GetJobPostDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var jobPost = await _jobPostService.GetByIdAsync(id);
        return Success(jobPost);
    }

    /// <summary>
    /// Retrieves all job posts
    /// </summary>
    /// <returns>A list of all job posts</returns>
    /// <response code="200">Returns the list of job posts</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<GetJobPostDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var jobPosts = await _jobPostService.GetAllAsync();
        return Success(jobPosts);
    }

    /// <summary>
    /// Retrieves a paginated list of job posts
    /// </summary>
    /// <param name="pageIndex">The page number (default: 1)</param>
    /// <param name="pageSize">The number of items per page (default: 10)</param>
    /// <returns>A paginated list of job posts with total count</returns>
    /// <response code="200">Returns the paginated job posts</response>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
    {
        var (items, totalCount) = await _jobPostService.GetPagedAsync(pageIndex, pageSize);
        return Success(new { items, totalCount, pageIndex, pageSize });
    }

    /// <summary>
    /// Creates a new job post
    /// </summary>
    /// <param name="dto">The job post data to create</param>
    /// <returns>The newly created job post</returns>
    /// <response code="201">Returns the newly created job post</response>
    /// <response code="400">If the request data is invalid</response>
    [HttpPost]
    [ProducesResponseType(typeof(GetJobPostDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateJobPostDto dto)
    {
        var jobPost = await _jobPostService.CreateAsync(dto);
        return Created(jobPost, "Job post created successfully");
    }


    /// <summary>
    /// Increments the view count of a job post
    /// </summary>
    /// <param name="id">The unique identifier of the job post</param>
    /// <returns>No content</returns>
    /// <response code="204">View count incremented successfully</response>
    /// <response code="404">If the job post is not found</response>
    [HttpPost("{id:guid}/view")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> IncrementView(Guid id)
    {
        await _jobPostService.IncrementViewCountAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Updates an existing job post by its identifier
    /// </summary>
    /// <param name="id">The unique identifier of the job post</param>
    /// <param name="dto">The updated job post data</param>
    /// <returns>The updated job post</returns>
    /// <response code="200">Returns the updated job post</response>
    /// <response code="400">If the ID in route doesn't match the ID in body</response>
    /// <response code="404">If the job post is not found</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(GetJobPostDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateJobPostDto dto)
    {
        if (id != dto.Id)
            return BadRequest("ID mismatch");

        var jobPost = await _jobPostService.UpdateAsync(dto);
        return Success(jobPost, "Job post updated successfully");
    }

    /// <summary>
    /// Soft deletes a job post by its identifier
    /// </summary>
    /// <param name="id">The unique identifier of the job post to delete</param>
    /// <returns>No content</returns>
    /// <response code="204">If the job post was successfully deleted</response>
    /// <response code="404">If the job post is not found</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _jobPostService.DeleteAsync(id);
        return NoContent("Job post deleted successfully");
    }
}