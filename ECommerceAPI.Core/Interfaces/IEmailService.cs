using ECommerceAPI.Core.Dtos;

namespace ECommerceAPI.Core.Interfaces;

/// <summary>
/// Interface for email service operations.
/// Provides methods for sending emails in the system.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an email asynchronously using the provided email message details
    /// </summary>
    /// <param name="emailMessage">The email message containing recipient, subject, and body</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task SendEmailAsync(EmailMessageDto emailMessage);
}
