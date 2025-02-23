using Microsoft.AspNetCore.Identity.UI.Services;

namespace Astronomic_Catalogs.Services;

public class DummyEmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        Console.WriteLine($"Email to: {email}\nSubject: {subject}\nMessage: {htmlMessage}");
        return Task.CompletedTask; 
    }
}
