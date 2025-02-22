using ECommerceAPI.Core.Dtos;

namespace ECommerceAPI.Core.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(EmailMessageDto emailMessage);
}
