using Application.DTOs.Emails;
using Application.Interfaces.Email;

namespace Infrastructure.Services.EmailServices
{
    public class EmailService : IEmailService
    {
        private readonly IEmailSender _emailSender;
        private readonly IEmailTemplate _emailTemplate;

        public EmailService(IEmailSender emailSender, IEmailTemplate emailTemplate)
        {
            _emailSender = emailSender;
            _emailTemplate = emailTemplate;
        }

        public async Task<bool> SendAsync(EmailMessage message, object model, CancellationToken cancellationToken = default)
        {
            EmailBody emailBody =  await _emailTemplate.CompileAsync(message.TemplateName, model);
            var compiledMessage = new CompiledEmailMessage(message.Subject, emailBody, message.To,
                message.Cc, message.Bcc, message.Attachments);
            
            return await _emailSender.SendAsync(compiledMessage, cancellationToken);
        }
    }
}