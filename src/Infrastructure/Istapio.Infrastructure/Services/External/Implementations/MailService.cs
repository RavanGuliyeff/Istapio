using AutoMapper.Internal;
using Istapio.Application.Models.DTOs.Mail;
using Istapio.Application.Models.Settings;
using Istapio.Application.Services.External.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Istapio.Infrastructure.Services.External.Implementations;

public class MailService : IMailService
{
    private readonly IConfiguration _config;
    private readonly MailSettings _emailSettings;
    public MailService(IConfiguration config, IOptions<MailSettings> emailSettings)
    {
        _config = config;
        _emailSettings = emailSettings.Value;
    }

    public async Task SendEmailsAsync(MailRequestDto dto)
    {
        var emailTasks = dto.ToEmails.Select(toEmail => Task.Run(async () =>
        {
            await SendEmailAsync(toEmail, dto.Subject, dto.Body, dto.Attachments);
        }));

        await Task.WhenAll(emailTasks);
    }
    private async Task SendEmailAsync(string toEmail, string subject, string body, List<IFormFile>? attachments)
    {
        using var smtpClient = new SmtpClient(_emailSettings.SmtpServer)
        {
            Port = int.Parse(_emailSettings.Port),
            Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
            EnableSsl = true
        };

        using var mailMessage = new MailMessage
        {
            From = new MailAddress(_emailSettings.From),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mailMessage.To.Add(toEmail);

        if (attachments != null && attachments.Any())
        {
            foreach (var file in attachments)
            {
                if (file.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    await file.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    var attachment = new Attachment(memoryStream, file.FileName);
                    mailMessage.Attachments.Add(attachment);
                }
            }
        }

        await smtpClient.SendMailAsync(mailMessage);

    }
}