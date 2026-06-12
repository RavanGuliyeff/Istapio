using Istapio.Application.Models.DTOs.Auth;
using Istapio.Application.Models.Responses;

namespace Istapio.Application.Services.Internal.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterDto dto);

    Task<AuthResponse> LoginAsync(LoginDto dto, string ipAddress);  
    Task VerifyEmailAsync(VerifyEmailDto dto);

    Task ForgotPasswordAsync(ForgotPasswordDto dto);

    Task ResetPasswordAsync(
        ResetPasswordDto dto,
        string ipAddress);
    Task<AuthResponse> RefreshTokenAsync(
        RefreshTokenDto dto,
        string ipAddress);

    Task LogoutAsync(
        RefreshTokenDto dto,
        string ipAddress);
}