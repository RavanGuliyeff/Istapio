using Istapio.Application.Exceptions;
using Istapio.Application.Models.DTOs.Auth;
using Istapio.Application.Models.DTOs.Mail;
using Istapio.Application.Models.Responses;
using Istapio.Application.Models.Settings;
using Istapio.Application.Services.External.Interfaces;
using Istapio.Application.Services.Internal.Interfaces;
using Istapio.Application.Utilities.Enums;
using Istapio.Application.Utilities.Helpers;
using Istapio.Domain.Entities;
using Istapio.Domain.Interfaces;
using Istapio.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Istapio.Application.Services.Internal.Implementations;
public sealed class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IOtpService _otpService;
    private readonly IMailService _emailService;
    private readonly IRefreshTokenRepository _refreshTokenRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly JwtSettings _jwt;

    public AuthService(
        UserManager<AppUser> userManager,
        ITokenService tokenService,
        IOtpService otpService,
        IMailService emailService,
        IRefreshTokenRepository refreshTokenRepo,
        IOptions<JwtSettings> jwtOptions,
        ICurrentUserService currentUserService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _otpService = otpService;
        _emailService = emailService;
        _refreshTokenRepo = refreshTokenRepo;
        _jwt = jwtOptions.Value;
        _currentUserService = currentUserService;
    }


    public async Task<UserProfileDto> GetProfileAsync()
    {
        var userId = _currentUserService.UserId;

        var user = await _userManager.Users.
            Include(u => u.Companies).
            Include(u => u.Skills).
                ThenInclude(us => us.Skill).
            FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new NotFoundException(nameof(AppUser), userId);

        return new UserProfileDto(
            Id: user.Id,
            Email: user.Email!,
            FirstName: user.FirstName,
            LastName: user.LastName,
            Created: user.Created,
            Roles: _currentUserService.Roles,
            Companies: user.Companies.Select(c => new GetUserCompanyDto(
                Id: c.Id,
                Name: c.Name,
                LogoUrl: c.LogoUrl)).ToList(),

            Skills: user.Skills.Select(s => new GetUserSkillDto(
                Id: s.SkillId,
                Name: s.Skill.Name)).ToList()
        );
    }

    public async Task<AuthResponse> RegisterAsync(
    RegisterDto dto)
    {
        if (await _userManager.FindByEmailAsync(dto.Email) is not null)
            throw new ConflictException("Email already exists.");

        var user = new AppUser
        {
            Email = dto.Email,
            UserName = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors
                .GroupBy(x => x.Code)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.Description).ToArray());

            throw new ValidationException(errors);
        }

        await _userManager.AddToRoleAsync(
            user,
            Domain.Constants.Roles.Member);

        var otp = await _otpService.GenerateAndStoreAsync(
            dto.Email,
            OtpType.EmailVerification);

        var emailDto = new MailRequestDto
        {
            ToEmails = [dto.Email],
            Subject = $"Verify your email: {otp}",
            Body = MailHelper.EmailVerificationOtp(dto.FirstName, otp),
            Attachments = null
        };
        await _emailService.SendEmailsAsync(emailDto);



        return new AuthResponse(
            AccessToken: string.Empty,
            RefreshToken: string.Empty,
            AccessTokenExpiresAt: DateTime.UtcNow,
            UserId: user.Id,
            Email: user.Email!,
            FirstName: user.FirstName,
            LastName: user.LastName,
            Roles: Array.Empty<string>()
        );
    }

    public async Task<AuthResponse> LoginAsync(
    LoginDto dto,
    string ipAddress)
    {
        var user = await _userManager.FindByEmailAsync(dto.EmailOrUsername)
            ?? await _userManager.FindByNameAsync(dto.EmailOrUsername)
            ?? throw new UnauthorizedException(
                "Login data is incorrect.");

        if (!await _userManager.CheckPasswordAsync(user, dto.Password))
            throw new UnauthorizedException(
                "Login data is incorrect.");


        if (!user.EmailConfirmed)
            throw new ValidationException(
                "Please verify your email first.");

        user.LastLoginIp = ipAddress;

        await _userManager.UpdateAsync(user);

        return await BuildAuthResponseAsync(
            user,
            ipAddress);
    }

    public async Task VerifyEmailAsync(
    VerifyEmailDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email)
            ?? throw new NotFoundException(
                nameof(AppUser),
                dto.Email);

        if (user.EmailConfirmed)
            throw new ConflictException(
                "Email already verified.");

        var isValid = await _otpService.VerifyAsync(
            dto.Email,
            dto.OtpCode,
            OtpType.EmailVerification);

        if (!isValid)
            throw new UnauthorizedException(
                "OTP code is invalid or expired.");

        user.EmailConfirmed = true;

        await _userManager.UpdateAsync(user);
    }
    public async Task ForgotPasswordAsync(
     ForgotPasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user is null)
            return;

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var resetLink = $"/reset-password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(dto.Email)}";

        var emailDto = new MailRequestDto
        {
            ToEmails = [dto.Email],
            Subject = "Reset your password",
            Body = MailHelper.PasswordReset(user.FirstName ?? dto.Email, resetLink),
            Attachments = null
        };
        await _emailService.SendEmailsAsync(emailDto);
    }

    public async Task ResetPasswordAsync(
    ResetPasswordDto dto,
    string ipAddress)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email)
            ?? throw new NotFoundException("User not found.");

        var result = await _userManager.ResetPasswordAsync(
            user,
            dto.Token,
            dto.NewPassword);

        if (!result.Succeeded)
        {
            var errors = result.Errors
                .GroupBy(x => x.Code)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.Description).ToArray());

            throw new ValidationException(errors);
        }

        await _refreshTokenRepo.RevokeAllUserTokensAsync(
            user.Id,
            "Password reset",
            ipAddress);
    }

    public async Task<AuthResponse> RefreshTokenAsync(
        RefreshTokenDto dto, string ipAddress)
    {
        var existingToken = await _refreshTokenRepo.GetActiveTokenAsync(dto.RefreshToken)
            ?? throw new UnauthorizedException("Token yanlis, müdd?ti bitib v? ya l?gv olunub.");

        var user = await _userManager.FindByIdAsync(existingToken.UserId)
            ?? throw new NotFoundException("Istifad?çi tapilmadi.");

        existingToken.RevokedAt = DateTime.UtcNow;
        existingToken.RevokedByIp = ipAddress;
        existingToken.ReasonRevoked = "Token rotasiyasi";

        var newRefreshToken = CreateRefreshToken(
            user.Id,
            ipAddress);

        existingToken.ReplacedByToken = newRefreshToken.Token;

        _refreshTokenRepo.Update(existingToken);
        await _refreshTokenRepo.AddAsync(newRefreshToken);
        await _refreshTokenRepo.SaveChangesAsync();

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _tokenService.GenerateAccessToken(user, roles);

        return BuildResponse(user, accessToken, newRefreshToken.Token, roles);
    }

    public async Task LogoutAsync(RefreshTokenDto dto, string ipAddress)
    {
        var token = await _refreshTokenRepo.GetActiveTokenAsync(dto.RefreshToken);
        if (token is null) return;

        token.RevokedAt = DateTime.UtcNow;
        token.RevokedByIp = ipAddress;
        token.ReasonRevoked = "Istifad?çi çixis etdi.";

        _refreshTokenRepo.Update(token);
        await _refreshTokenRepo.SaveChangesAsync();
    }


    private async Task<AuthResponse> BuildAuthResponseAsync(
    AppUser user,
    string ipAddress)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var accessToken = _tokenService.GenerateAccessToken(
            user,
            roles);

        var refreshToken = CreateRefreshToken(
            user.Id,
            ipAddress);

        await _refreshTokenRepo.AddAsync(
            refreshToken);

        await _refreshTokenRepo.SaveChangesAsync();

        return BuildResponse(
            user,
            accessToken,
            refreshToken.Token,
            roles);
    }

    private RefreshToken CreateRefreshToken(
    string userId,
    string ipAddress)
    {
        var refreshDays = _jwt.RefreshTokenExpiresInDays;

        return new RefreshToken
        {
            Token = _tokenService.GenerateRefreshToken(),
            UserId = userId,
            CreatedByIp = ipAddress,
            ExpiresAt = DateTime.UtcNow.AddDays(refreshDays)
        };
    }

    private static AuthResponse BuildResponse(
        AppUser user, string accessToken, string refreshToken, IList<string> roles)
        => new(
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            AccessTokenExpiresAt: DateTime.UtcNow.AddMinutes(15),
            UserId: user.Id,
            Email: user.Email!,
            FirstName: user.FirstName,
            LastName: user.LastName,
            Roles: roles.ToList().AsReadOnly()
        );


}


