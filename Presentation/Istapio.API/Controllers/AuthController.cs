using Istapio.API.Controllers.Common;
using Istapio.Application.Models.DTOs.Auth;
using Istapio.Application.Models.Responses;
using Istapio.Application.Services.Internal.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Istapio.API.Controllers;


/// <summary>
/// Controller for authentication and authorization operations
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AuthController : BaseController
{
    private readonly IAuthService _authService;

    /// <summary>
    /// Initializes a new instance of the AuthController
    /// </summary>
    /// <param name="authService">The auth service instance</param>
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Registers a new user
    /// </summary>
    /// <param name="dto">Registration data</param>
    /// <returns>Registration result</returns>
    /// <response code="201">User registered successfully</response>
    /// <response code="400">Invalid request data</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        return Created(result, "Registration completed successfully. Please verify your email.");
    }

    /// <summary>
    /// Authenticates a user
    /// </summary>
    /// <param name="dto">Login credentials</param>
    /// <returns>Authentication result</returns>
    /// <response code="200">Login successful</response>
    /// <response code="401">Invalid credentials</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var ipAddress = GetClientIpAddress() ?? "Unknown";

        var result = await _authService.LoginAsync(dto, ipAddress);

        return Success(result, "Login successful");
    }

    /// <summary>
    /// Verifies user email
    /// </summary>
    /// <param name="dto">Verification data</param>
    /// <returns>Success response</returns>
    /// <response code="200">Email verified successfully</response>
    /// <response code="400">Invalid OTP code</response>
    [HttpPost("verify-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto dto)
    {
        await _authService.VerifyEmailAsync(dto);

        return Success(new { }, "Email verified successfully");
    }

    /// <summary>
    /// Sends password reset email
    /// </summary>
    /// <param name="dto">Forgot password request</param>
    /// <returns>Success response</returns>
    /// <response code="200">Reset email sent</response>
    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        await _authService.ForgotPasswordAsync(dto);

        return Success(new { }, "Password reset instructions sent successfully");
    }

    /// <summary>
    /// Resets user password
    /// </summary>
    /// <param name="dto">Password reset data</param>
    /// <returns>Success response</returns>
    /// <response code="200">Password reset successfully</response>
    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _authService.ResetPasswordAsync(dto, ipAddress);

        return Success(new { }, "Password reset successfully");
    }

    /// <summary>
    /// Refreshes access token
    /// </summary>
    /// <param name="dto">Refresh token data</param>
    /// <returns>New access and refresh tokens</returns>
    /// <response code="200">Token refreshed successfully</response>
    /// <response code="401">Invalid refresh token</response>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        var result = await _authService.RefreshTokenAsync(dto, ipAddress);

        return Success(result, "Token refreshed successfully");
    }

    /// <summary>
    /// Logs out the current user
    /// </summary>
    /// <param name="dto">Refresh token data</param>
    /// <returns>No content</returns>
    /// <response code="204">Logout successful</response>
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenDto dto)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _authService.LogoutAsync(dto, ipAddress);

        return NoContent("Logout successful");
    }

    /// <summary>
    /// Returns the profile of the currently authenticated user
    /// </summary>
    /// <returns>User profile data</returns>
    /// <response code="200">Profile retrieved successfully</response>
    /// <response code="401">User is not authenticated</response>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetProfile()
    {
        var result = await _authService.GetProfileAsync();
        return Success(result, "Profile retrieved successfully");
    }
}