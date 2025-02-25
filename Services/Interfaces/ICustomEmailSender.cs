using Microsoft.AspNetCore.Identity.UI.Services;

namespace Astronomic_Catalogs.Services.Interfaces;

public interface ICustomEmailSender : IEmailSender
{
    Task SendEmailAsync(string email, string subject, string htmlMessage, string? userId);
}

