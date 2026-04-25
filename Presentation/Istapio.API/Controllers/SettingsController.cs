using Microsoft.AspNetCore.Mvc;
using Istapio.API.Controllers.Common;
using Istapio.Application.Models.DTOs.Setting;
using Istapio.Application.Services.Internal.Interfaces;

namespace Istapio.API.Controllers;

/// <summary>
/// Controller for managing application settings
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class SettingsController : BaseController
{
    private readonly ISettingService _settingService;

    /// <summary>
    /// Initializes a new instance of the SettingsController
    /// </summary>
    /// <param name="settingService">The setting service instance</param>
    public SettingsController(ISettingService settingService)
    {
        _settingService = settingService;
    }

    /// <summary>
    /// Retrieves a setting by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the setting</param>
    /// <returns>The requested setting</returns>
    /// <response code="200">Returns the setting</response>
    /// <response code="404">If the setting is not found</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GetSettingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var setting = await _settingService.GetByIdAsync(id);
        return Success(setting);
    }

    /// <summary>
    /// Retrieves a setting by its key
    /// </summary>
    /// <param name="key">The unique key of the setting</param>
    /// <returns>The requested setting</returns>
    /// <response code="200">Returns the setting</response>
    /// <response code="404">If the setting is not found</response>
    [HttpGet("by-key/{key}")]
    [ProducesResponseType(typeof(GetSettingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByKey(string key)
    {
        var setting = await _settingService.GetByKeyAsync(key);
        return Success(setting);
    }

    /// <summary>
    /// Retrieves all settings
    /// </summary>
    /// <returns>A list of all settings</returns>
    /// <response code="200">Returns the list of settings</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<GetSettingDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var settings = await _settingService.GetAllAsync();
        return Success(settings);
    }

    /// <summary>
    /// Retrieves a paginated list of settings
    /// </summary>
    /// <param name="pageIndex">The page number (default: 1)</param>
    /// <param name="pageSize">The number of items per page (default: 10)</param>
    /// <returns>A paginated list of settings with total count</returns>
    /// <response code="200">Returns the paginated settings</response>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
    {
        var (items, totalCount) = await _settingService.GetPagedAsync(pageIndex, pageSize);
        return Success(new { items, totalCount, pageIndex, pageSize });
    }

    /// <summary>
    /// Creates a new setting
    /// </summary>
    /// <param name="dto">The setting data to create</param>
    /// <returns>The newly created setting</returns>
    /// <response code="201">Returns the newly created setting</response>
    /// <response code="400">If the request data is invalid</response>
    /// <response code="409">If a setting with the same key already exists</response>
    [HttpPost]
    [ProducesResponseType(typeof(GetSettingDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateSettingDto dto)
    {
        var setting = await _settingService.CreateAsync(dto);
        return Created(setting, "Setting created successfully");
    }

    /// <summary>
    /// Updates an existing setting by its identifier
    /// </summary>
    /// <param name="id">The unique identifier of the setting</param>
    /// <param name="dto">The updated setting data</param>
    /// <returns>The updated setting</returns>
    /// <response code="200">Returns the updated setting</response>
    /// <response code="400">If the ID in route doesn't match the ID in body</response>
    /// <response code="404">If the setting is not found</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(GetSettingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSettingDto dto)
    {
        if (id != dto.Id)
            return BadRequest("ID mismatch");

        var setting = await _settingService.UpdateAsync(dto);
        return Success(setting, "Setting updated successfully");
    }

    /// <summary>
    /// Updates an existing setting by its key
    /// </summary>
    /// <param name="key">The unique key of the setting</param>
    /// <param name="dto">The updated setting data</param>
    /// <returns>The updated setting</returns>
    /// <response code="200">Returns the updated setting</response>
    /// <response code="400">If the key in route doesn't match the key in body</response>
    /// <response code="404">If the setting is not found</response>
    [HttpPut("by-key/{key}")]
    [ProducesResponseType(typeof(GetSettingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateByKey(string key, [FromBody] UpdateSettingDto dto)
    {
        if (key != dto.Key)
            return BadRequest("Key mismatch");

        var setting = await _settingService.UpdateByKeyAsync(dto);
        return Success(setting, "Setting updated successfully");
    }

    /// <summary>
    /// Soft deletes a setting by its identifier
    /// </summary>
    /// <param name="id">The unique identifier of the setting to delete</param>
    /// <returns>No content</returns>
    /// <response code="204">If the setting was successfully deleted</response>
    /// <response code="404">If the setting is not found</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _settingService.DeleteAsync(id);
        return NoContent("Setting deleted successfully");
    }

    /// <summary>
    /// Soft deletes a setting by its key
    /// </summary>
    /// <param name="key">The unique key of the setting to delete</param>
    /// <returns>No content</returns>
    /// <response code="204">If the setting was successfully deleted</response>
    /// <response code="404">If the setting is not found</response>
    [HttpDelete("by-key/{key}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteByKey(string key)
    {
        await _settingService.DeleteByKeyAsync(key);
        return NoContent("Setting deleted successfully");
    }

    /// <summary>
    /// Retrieves only the value of a setting by its key
    /// </summary>
    /// <param name="key">The unique key of the setting</param>
    /// <returns>The setting value as a string</returns>
    /// <response code="200">Returns the setting value</response>
    /// <response code="404">If the setting is not found</response>
    [HttpGet("value/{key}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetValue(string key)
    {
        var value = await _settingService.GetValueAsync(key);
        if (value == null)
            return NotFound($"Setting with key '{key}' not found");

        return Success(value);
    }

    /// <summary>
    /// Sets or updates a setting value by its key (upsert operation)
    /// </summary>
    /// <param name="key">The unique key of the setting</param>
    /// <param name="request">The value to set</param>
    /// <returns>Success message</returns>
    /// <response code="200">If the setting value was successfully set or updated</response>
    [HttpPost("value/{key}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SetValue(string key, [FromBody] SetValueRequest request)
    {
        await _settingService.SetValueAsync(key, request.Value);
        return Success(new { key, value = request.Value }, "Setting value set successfully");
    }
}

/// <summary>
/// Request model for setting a value
/// </summary>
public record SetValueRequest(string Value);