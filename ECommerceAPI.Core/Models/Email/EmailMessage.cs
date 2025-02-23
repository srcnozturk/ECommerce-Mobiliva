namespace ECommerceAPI.Core.Models.Email;

/// <summary>
/// Represents an email message
/// </summary>
public class EmailMessage
{
    /// <summary>
    /// Gets or sets the recipient's email address
    /// </summary>
    public string To { get; set; }

    /// <summary>
    /// Gets or sets the email subject
    /// </summary>
    public string Subject { get; set; }

    /// <summary>
    /// Gets or sets the email body content
    /// </summary>
    public string Body { get; set; }
}
