namespace Istapio.Application.Utilities.Helpers;

public static class MailHelper
{
    public static string EmailVerificationOtp(string firstName, string otp) => $"""
        <html><body style="font-family:Arial,sans-serif;background:#f4f4f4;padding:30px;">
          <div style="max-width:480px;margin:auto;background:#fff;border-radius:8px;padding:32px;">
            <h2 style="color:#2d2d2d;">Hi, {firstName}!</h2>
            <p style="color:#555;">Use the code below to verify your email address:</p>
            <div style="font-size:36px;font-weight:bold;letter-spacing:8px;color:#4f46e5;
                        text-align:center;padding:20px 0;">{otp}</div>
            <p style="color:#999;font-size:12px;">This code is valid for 10 minutes. If you did not request this, please ignore this email.</p>
          </div>
        </body></html>
        """;

    public static string PasswordReset(string firstName, string resetLink) => $"""
        <html><body style="font-family:Arial,sans-serif;background:#f4f4f4;padding:30px;">
          <div style="max-width:480px;margin:auto;background:#fff;border-radius:8px;padding:32px;">
            <h2 style="color:#2d2d2d;">Hi, {firstName}!</h2>
            <p style="color:#555;">We received a request to reset your password. Click the button below to proceed:</p>
            <a href="{resetLink}"
               style="display:inline-block;margin-top:16px;padding:12px 28px;
                      background:#4f46e5;color:#fff;border-radius:6px;
                      text-decoration:none;font-weight:bold;">
              Reset Password
            </a>
            <p style="color:#999;font-size:12px;margin-top:20px;">
              This link expires in 1 hour. If you did not request a password reset, you can safely ignore this email.
            </p>
          </div>
        </body></html>
        """;

    public static string WelcomeEmail(string firstName) => $"""
        <html><body style="font-family:Arial,sans-serif;background:#f4f4f4;padding:30px;">
          <div style="max-width:480px;margin:auto;background:#fff;border-radius:8px;padding:32px;">
            <h2 style="color:#2d2d2d;">Welcome, {firstName}!</h2>
            <p style="color:#555;">Thank you for joining Istapio.</p>
            <p style="color:#555;">Your account has been successfully verified. You can now access all features.</p>
          </div>
        </body></html>
        """;

    public static string AccountLocked(string firstName) => $"""
        <html><body style="font-family:Arial,sans-serif;background:#f4f4f4;padding:30px;">
          <div style="max-width:480px;margin:auto;background:#fff;border-radius:8px;padding:32px;">
            <h2 style="color:#c0392b;">Account Locked</h2>
            <p style="color:#555;">Hi {firstName}, your account has been temporarily locked due to multiple failed login attempts.</p>
            <p style="color:#555;">Please contact our support team for assistance.</p>
          </div>
        </body></html>
        """;
}
