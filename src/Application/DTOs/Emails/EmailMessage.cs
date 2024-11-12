namespace Application.DTOs.Emails
{
    public record EmailMessage(
        string Subject,
        string TemplateName,
        IEnumerable<EmailAddress> To,
        IEnumerable<EmailAddress>? Cc = null,
        IEnumerable<EmailAddress>? Bcc = null,
        IEnumerable<EmailAttachment>? Attachments = null);
}