using Application.DTOs.Emails;

namespace Application.Interfaces.Email
{
    public interface IEmailService
    {
        Task<bool> SendAsync(EmailMessage message, object model, CancellationToken cancellationToken = default);
    }
}