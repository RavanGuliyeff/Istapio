namespace Istapio.Domain.Constants;

public static class CacheKeys
{
    public static string EmailVerifyOtp(string email) => $"otp:verify-email:{email.ToLowerInvariant()}";
    public static string PasswordResetOtp(string email) => $"otp:reset-password:{email.ToLowerInvariant()}";

    public static string OtpDailyRate(string email) => $"otp:rate:{email.ToLowerInvariant()}";

    public static string OtpFailCount(string email) => $"otp:fail:{email.ToLowerInvariant()}";
}