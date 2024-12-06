using Application.DTOs.Emails;
using Application.Interfaces.Email;
using Application.Users.Commands.ResetPassword;
using Infrastructure.RazorTemplates.EmailTemplates.Shared;

namespace Infrastructure.Services.Email
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

        public async Task<bool> SendEmailConfirmationLinkAsync(EmailMessage message, string confirmationLink, CancellationToken cancellationToken = default)
        {
            EmailConfirmationViewModel model = new(confirmationLink);
            return await SendAsync(message, model, cancellationToken);
        }

        public async Task<bool> SendPasswordResetLinkAsync(EmailMessage message, string resetLink, CancellationToken cancellationToken = default)
        {
            ResetPasswordViewModel viewModel = new(resetLink);
            return await SendAsync(message, viewModel, cancellationToken);
        }
        
        private async Task<bool> SendAsync(EmailMessage message, object model, CancellationToken cancellationToken = default)
        {
            EmailBody emailBody =  await _emailTemplate.CompileAsync(message.TemplateName, model);
            CompiledEmailMessage compiledMessage = new(message.Subject, emailBody, message.To,
                message.Cc, message.Bcc, message.Attachments);
            
            return await _emailSender.SendAsync(compiledMessage, cancellationToken);
        }
    }
}