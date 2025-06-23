using Application.DTOs.Emails;

namespace Application.Interfaces.Email;

public interface IEmailService
{
    Task<bool> SendEmailConfirmationLinkAsync(EmailMessage message, string confirmationLink, int retries = 0, CancellationToken cancellationToken = default);
    Task<bool> SendPasswordResetLinkAsync(EmailMessage message, string resetLink, int retries = 0, CancellationToken cancellationToken = default);
    Task<bool> SendCardBlockedAsync(EmailMessage message, string ownerName, string cardNumber, int retries = 0, CancellationToken cancellationToken = default);
}