using Application.DTOs.Emails;
using Application.Interfaces.Email;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Infrastructure.Services.EmailServices
{
    public class MailkitSender : IEmailSender
    {
        private readonly EmailOptions _emailOptions;

        public MailkitSender(IOptions<EmailOptions> emailOptions)
        {
            _emailOptions = emailOptions.Value;
        }
        
        public async Task<bool> SendAsync(CompiledEmailMessage message, CancellationToken cancellationToken = default)
        {
            var mimeMessage = new MimeMessage();

            mimeMessage.Subject = message.Subject;
            mimeMessage.From.Add(new MailboxAddress(_emailOptions.SenderName, _emailOptions.SenderEmail));
            mimeMessage.To.AddRange(message.To.Select(t => new MailboxAddress(t.DisplayName, t.Address)));
            
            var builder = new BodyBuilder();

            builder.TextBody = message.Body.PlainText;
            builder.HtmlBody = message.Body.Html;
            
            if (message.Cc != null && message.Cc.Any())
                mimeMessage.Cc.AddRange(message.Cc.Select(c => new MailboxAddress(c.DisplayName, c.Address)));
            
            if (message.Bcc != null && message.Bcc.Any())
                mimeMessage.Cc.AddRange(message.Bcc.Select(b => new MailboxAddress(b.DisplayName, b.Address)));

            if (message.Attachments != null && message.Attachments.Any())
            {
                foreach (var attachment in message.Attachments)
                    await builder.Attachments.AddAsync(attachment.FileName, attachment.StreamFactory.Invoke(), cancellationToken);
            }

            mimeMessage.Body = builder.ToMessageBody();

            return await SendMimeMessageAsync(mimeMessage, cancellationToken);
        }

        private async Task<bool> SendMimeMessageAsync(MimeMessage message, CancellationToken cancellationToken)
        {
            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(_emailOptions.SmtpServer, _emailOptions.Port, _emailOptions.UseSsl, cancellationToken);
            await smtp.AuthenticateAsync(_emailOptions.SenderEmail, _emailOptions.Password, cancellationToken);
            await smtp.SendAsync(message, cancellationToken);
            await smtp.DisconnectAsync(true, cancellationToken);

            return true;
        }
    }
}