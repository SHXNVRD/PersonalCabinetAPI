using System.Xml.Serialization;
using Application.DTOs.Emails;

namespace Application.Interfaces.Email;

public interface IEmailSender
{
    Task<bool> SendAsync(CompiledEmailMessage message, int retries = 0, CancellationToken cancellationToken = default);
}