using Application.DTOs.Emails;

namespace Application.Interfaces.Email
{
    public interface IEmailTemplate
    {
        Task<EmailBody> CompileAsync(string templateName, object model);
    }
}