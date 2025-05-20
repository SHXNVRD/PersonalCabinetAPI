using Application.DTOs.Emails;
using Application.Interfaces.Email;
using Razor.Templating.Core;
using Templates.EmailTemplates;

namespace Infrastructure.Email;

public class EmailService : IEmailService
{
    private readonly IEmailSender _emailSender;
    private readonly IRazorTemplateEngine _engine;

    public EmailService(IEmailSender emailSender, IRazorTemplateEngine engine)
    {
        _emailSender = emailSender;
        _engine = engine;
    }

    public async Task<bool> SendEmailConfirmationLinkAsync(EmailMessage message, string confirmationLink, int retries = 0, CancellationToken cancellationToken = default)
    {
        EmailConfirmationViewModel model = new(confirmationLink);
        return await SendAsync(message, TemplateKeys.EmailConfirmationTemplateKey, model, retries, cancellationToken);
    }

    public async Task<bool> SendPasswordResetLinkAsync(EmailMessage message, string resetLink, int retries = 0, CancellationToken cancellationToken = default)
    {
        ResetPasswordViewModel viewModel = new(resetLink);
        return await SendAsync(message, TemplateKeys.PasswordResetTemplateKey, viewModel, retries, cancellationToken);
    }

    public async Task<bool> SendCardBlockedAsync(EmailMessage message, string ownerName, string cardNumber, int retries = 0,  CancellationToken cancellationToken = default)
    {
        CardBlockedViewModel viewModel = new(ownerName, cardNumber);
        return await SendAsync(message, TemplateKeys.CardBlockedTemplateKey, viewModel, retries, cancellationToken);
    }

    private async Task<bool> SendAsync(EmailMessage message, string templateName, object model, int retries = 0, CancellationToken cancellationToken = default)
    {
        var renderResult = await _engine.TryRenderAsync(templateName, model);
        if (!renderResult.ViewExists)
            return false;

        var emailBody = new EmailBody(renderResult.RenderedView!, string.Empty);
        CompiledEmailMessage compiledMessage = new(message.Subject, emailBody, message.To,
            message.Cc, message.Bcc, message.Attachments);
            
        return await _emailSender.SendAsync(compiledMessage, retries, cancellationToken);
    }
}