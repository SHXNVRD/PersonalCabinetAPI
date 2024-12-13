using Application.DTOs.Emails;
using Application.Interfaces;
using Application.Interfaces.Email;
using Application.Services;
using FluentResults;
using MediatR;

namespace Application.Users.Commands.CreateEmailConfirmationLink
{
    public class CreateEmailConfirmationLinkCommandHandler : IRequestHandler<CreateEmailConfirmationLinkCommand, Result>
    {
        private readonly IEmailService _emailService;
        private readonly ILinkService _linkService;
        private readonly AppUserManager _userManager;

        public CreateEmailConfirmationLinkCommandHandler(
            IEmailService emailService, 
            ILinkService linkService,
            AppUserManager userManager)
        {
            _emailService = emailService;
            _linkService = linkService;
            _userManager = userManager;
        }

        public async Task<Result> Handle(CreateEmailConfirmationLinkCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
                return Result.Fail("User with specified email not found");
            
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
   
            //([request.Email]) См. переопределение implicit оператора EmailAddress
            var message = new EmailMessage("Подтверждение регистрации", "EmailConfirmationMonolith", [request.Email]);

            string confirmationLink = _linkService.GetUriByAction(
                "ConfirmEmail",
                "Account", 
                new { email = user.Email, token })!;

            var isMessageSent = await _emailService.SendEmailConfirmationLinkAsync(message, confirmationLink, cancellationToken);

            return Result.OkIf(isMessageSent, "Fail to sent confirmation link");
        }
    }
}