using Microsoft.AspNetCore.Http;

namespace Istapio.Application.Models.DTOs.Mail
{
    public sealed record MailRequestDto
    {
        public List<string> ToEmails { get; set; } = default!;
        public string Subject { get; set; } = default!;
        public string Body { get; set; } = default!;
        public List<IFormFile>? Attachments { get; set; }
    }
}
