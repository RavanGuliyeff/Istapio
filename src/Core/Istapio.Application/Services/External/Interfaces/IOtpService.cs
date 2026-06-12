using Istapio.Application.Utilities.Enums;

namespace Istapio.Application.Services.External.Interfaces;

public interface IOtpService
{
    Task<string> GenerateAndStoreAsync(string email, OtpType type, CancellationToken ct = default);
    Task<bool> VerifyAsync(string email, string code, OtpType type, CancellationToken ct = default);
    Task InvalidateAsync(string email, OtpType type, CancellationToken ct = default);
}