using Istapio.Domain.Entities;

namespace Istapio.Application.Services.External.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(AppUser user, IList<string> roles);
    string GenerateRefreshToken();
    string GenerateSecureOtp();
    string GenerateSecureToken();
}
