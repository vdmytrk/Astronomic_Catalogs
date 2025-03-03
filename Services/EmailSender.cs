using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Models.Services;
using Astronomic_Catalogs.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Astronomic_Catalogs.Services;

public class EmailSender : ICustomEmailSender
{
    private readonly AuthMessageSenderOptions _options;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(
        IOptions<AuthMessageSenderOptions> options, 
        ApplicationDbContext context, 
        ILogger<EmailSender> logger
        )
    {
        _options = options.Value;
        _context = context;
        _logger = logger;
    }


    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        using (var client = new SmtpClient("smtp.gmail.com", 587))
        {
            client.Credentials = new NetworkCredential(_options.Email, _options.Password);
            client.EnableSsl = true;

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_options.Email),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);            

            try
            {
                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred during sending the email to {_options.Email}.");
                throw;
            }
        }
    }

    /// <summary>
    /// DV: To count the number of registration email sent. 
    /// </summary>
    /// <param name="email"></param>
    /// <param name="subject"></param>
    /// <param name="htmlMessage"></param>
    /// <param name="userId">For registration email</param>
    /// <param name="registrationEmail"></param>
    /// <returns></returns>
    public async Task SendEmailAsync(string email, string subject, string htmlMessage, string? userId = null)
    {
        using (var client = new SmtpClient("smtp.gmail.com", 587))
        {
            client.Credentials = new NetworkCredential(_options.Email, _options.Password);
            client.EnableSsl = true;

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_options.Email),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);
                        
            var aspNetUser = await _context.Users.FindAsync(userId);
            if (aspNetUser is not null)
            {
                aspNetUser.LastRegisterEmailSent = DateTime.UtcNow;
                aspNetUser.CountRegisterEmailSent += 1;
                _context.Update(aspNetUser);
                await _context.SaveChangesAsync();
            }

            try
            {
                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred during sending the email to {_options.Email}.");
                throw;
            }
        }
    }
}

