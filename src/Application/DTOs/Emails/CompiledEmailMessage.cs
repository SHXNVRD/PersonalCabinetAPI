namespace Application.DTOs.Emails
{
    public record CompiledEmailMessage(
        string Subject,
        EmailBody Body,
        IEnumerable<EmailAddress> To,
        IEnumerable<EmailAddress>? Cc = null,
        IEnumerable<EmailAddress>? Bcc = null,
        IEnumerable<EmailAttachment>? Attachments = null);
}