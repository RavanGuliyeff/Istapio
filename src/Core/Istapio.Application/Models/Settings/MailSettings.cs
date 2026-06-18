namespace Istapio.Application.Models.Settings
{
    public sealed class MailSettings
    {
        public string SmtpServer { get; set; } = null!;
        public string Port { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string From { get; set; } = null!;
    }
}
