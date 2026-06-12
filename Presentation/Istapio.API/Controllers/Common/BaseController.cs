using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Istapio.Application.Models.Responses;

namespace Istapio.API.Controllers.Common;

[Route("api/[controller]")]
[ApiController]
public class BaseController : ControllerBase
{
    protected IActionResult Success<T>(T data, string? message = null)
    {
        return Ok(ApiResponse<T>.SuccessResponse(data, 200, message));
    }

    protected IActionResult Created<T>(T data, string? message = null)
    {
        return StatusCode(201, ApiResponse<T>.SuccessResponse(data, 201, message));
    }

    protected IActionResult Accepted<T>(T data, string? message = null)
    {
        return StatusCode(202, ApiResponse<T>.SuccessResponse(data, 202, message));
    }

    protected IActionResult NoContent(string? message = null)
    {
        return StatusCode(204, ApiResponse.SuccessResponse(new { }, 204, message));
    }


    protected string GetClientIpAddress()
    {
        var forwardedFor = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(forwardedFor))
            return forwardedFor.Split(',')[0].Trim();

        var realIp = HttpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(realIp))
            return realIp;

        var remoteIp = HttpContext.Connection.RemoteIpAddress;

        if (remoteIp != null && remoteIp.IsIPv4MappedToIPv6)
            remoteIp = remoteIp.MapToIPv4();

        return remoteIp?.ToString() ?? "Unknown";

    }

}
