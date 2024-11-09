using Application.DTOs.Emails;
using Application.Interfaces.Email;
using Application.Services;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Application.Users.Commands.CreateEmailConfirmationLink
{
    public class CreateEmailConfirmationLinkCommandHandler : IRequestHandler<CreateEmailConfirmationLinkCommand, Result>
    {
        private readonly IEmailService _emailService;
        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppUserManager _userManager;

        public CreateEmailConfirmationLinkCommandHandler(
            IEmailService emailService, 
            LinkGenerator linkGenerator, 
            IHttpContextAccessor httpContextAccessor, AppUserManager userManager)
        {
            _emailService = emailService;
            _linkGenerator = linkGenerator;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<Result> Handle(CreateEmailConfirmationLinkCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
                return Result.Fail("User with specified email not found");
            
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
   
            //([request.Email]) См. переопределение implict оператора EmailMessage
            var message = new EmailMessage("Подтверждение регистрации", "EmailConfirmation", [request.Email]);
            object model = new
            {
                ConfirmationLink = _linkGenerator.GetUriByAction(_httpContextAccessor.HttpContext!, "ConfirmEmail", "Account", new { email = user.Email, token})
            };

            var messageSented = await _emailService.SendAsync(message, model, cancellationToken);

            return Result.OkIf(messageSented, "Fail to sent confirmation link");
        }
    }
}