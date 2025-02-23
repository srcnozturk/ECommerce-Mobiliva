using System.Net;
using System.Net.Mail;

namespace ECommerceAPI.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(EmailMessage emailMessage)
    {
        var smtpClient = new SmtpClient(_configuration["Email:SmtpServer"])
        {
            Port = int.Parse(_configuration["Email:Port"]),
            Credentials = new NetworkCredential(
                  _configuration["Email:Username"],
                  _configuration["Email:Password"]
              ),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_configuration["Email:Username"]),
            Subject = emailMessage.Subject,
            Body = emailMessage.Body,
            IsBodyHtml = true
        };
        mailMessage.To.Add(emailMessage.To);

        await smtpClient.SendMailAsync(mailMessage);
    }
}
