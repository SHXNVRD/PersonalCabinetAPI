namespace Application.DTOs.Emails
{
    public record EmailAttachment(
        string FileName,
        Func<Stream> StreamFactory);
}