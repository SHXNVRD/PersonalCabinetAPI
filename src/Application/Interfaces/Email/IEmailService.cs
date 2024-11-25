using Application.DTOs.Emails;

namespace Application.Interfaces.Email
{
    public interface IEmailService
    {
        Task<bool> SendEmailConfirmationLinkAsync(EmailMessage message, string confirmationLink, CancellationToken cancellationToken = default);
    }
}