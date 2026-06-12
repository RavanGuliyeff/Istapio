using AutoMapper.Internal;
using Istapio.Application.Models.DTOs.Mail;

namespace Istapio.Application.Services.External.Interfaces;

public interface IMailService
{
    Task SendEmailsAsync(MailRequestDto dto);
}