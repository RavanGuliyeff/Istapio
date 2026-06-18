using Istapio.Application.Exceptions;
using Istapio.Application.Models.DTOs.Cache;
using Istapio.Application.Services.External.Interfaces;
using Istapio.Application.Utilities.Constants;
using Istapio.Application.Utilities.Enums;
using Istapio.Domain.Constants;

namespace Istapio.Infrastructure.Services.External.Implementations;

public sealed class OtpService : IOtpService
{
    private const byte MaxDailyOtp = 5;
    private const byte MaxFailCount = 5;
    private static readonly TimeSpan OtpTtl = TimeSpan.FromMinutes(3);
    private static readonly TimeSpan FailTtl = TimeSpan.FromMinutes(15);

    private readonly ICacheService _cache;
    private readonly ITokenService _tokenService;

    public OtpService(ICacheService cache, ITokenService tokenService)
    {
        _cache = cache;
        _tokenService = tokenService;
    }

    public async Task<string> GenerateAndStoreAsync(
        string email, OtpType type, CancellationToken ct = default)
    {
        var rateKey = CacheKeys.Otp.DailyRate(email);
        var sentToday = await _cache.GetAsync<byte>(rateKey, ct);
        if (sentToday >= MaxDailyOtp)
            throw new TooManyRequestsException("Bu gün maksimum OTP limitinə çatdınız.");

        var code = _tokenService.GenerateSecureOtp();
        var entry = new OtpEntry(code, DateTime.UtcNow.Add(OtpTtl), DateTime.UtcNow);

        var otpKey = GetOtpKey(email, type);
        await _cache.SetAsync(otpKey, entry, OtpTtl, ct);

        var midnight = TimeSpan.FromTicks(
            DateTime.UtcNow.Date.AddDays(1).Ticks - DateTime.UtcNow.Ticks);
        await _cache.SetAsync(rateKey, (byte)(sentToday + 1), midnight, ct);

        return code;
    }

    public async Task<bool> VerifyAsync(
        string email, string code, OtpType type, CancellationToken ct = default)
    {
        var failKey = CacheKeys.Otp.FailCount(email);
        var failCount = await _cache.GetAsync<byte>(failKey, ct);
        if (failCount >= MaxFailCount)
            throw new TooManyRequestsException("Çox sayda yanlış cəhd. 15 dəqiqə sonra yenidən cəhd edin.");

        var otpKey = GetOtpKey(email, type);
        var entry = await _cache.GetAsync<OtpEntry>(otpKey, ct);

        if (entry is null || entry.IsExpired)
        {
            await IncrementFailCountAsync(failKey, failCount, ct);
            return false;
        }

        var isValid = CryptographicEquals(entry.Code, code);

        if (!isValid)
        {
            await IncrementFailCountAsync(failKey, failCount, ct);
            return false;
        }

        await _cache.RemoveAsync(otpKey, ct);
        await _cache.RemoveAsync(failKey, ct);
        return true;
    }

    public async Task InvalidateAsync(string email, OtpType type, CancellationToken ct = default)
    {
        await _cache.RemoveAsync(GetOtpKey(email, type), ct);
    }


    private static string GetOtpKey(string email, OtpType type) => type switch
    {
        OtpType.EmailVerification => CacheKeys.Otp.EmailVerify(email),
        OtpType.PasswordReset => CacheKeys.Otp.PasswordReset(email),
        _ => throw new ArgumentOutOfRangeException(nameof(type))
    };

    private async Task IncrementFailCountAsync(
        string failKey, byte current, CancellationToken ct)
    {
        await _cache.SetAsync(failKey, (byte)(current + 1), FailTtl, ct);
    }

    private static bool CryptographicEquals(string a, string b)
    {
        if (a.Length != b.Length) return false;
        var diff = 0;
        for (var i = 0; i < a.Length; i++)
            diff |= a[i] ^ b[i];
        return diff == 0;
    }
}
