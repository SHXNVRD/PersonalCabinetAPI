using Application.DTOs.Emails;
using Application.Interfaces.Email;
using Application.Services;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;

namespace Application.Users.Commands.SendPasswordResetLink;

public class SendPasswordResetLinkCommandHandler : IRequestHandler<SendPasswordResetLinkCommand, Result>
{
    private readonly IEmailService _emailService;
    private readonly LinkGenerator _linkGenerator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AppUserManager _userManager;

    public SendPasswordResetLinkCommandHandler(
        AppUserManager userManager,
        IEmailService emailService,
        LinkGenerator linkGenerator, 
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _emailService = emailService;
        _linkGenerator = linkGenerator;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result> Handle(SendPasswordResetLinkCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Result.Fail("User with specified email not found");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var resetPasswordLink = _linkGenerator.GetUriByAction(
            _httpContextAccessor.HttpContext!,
            "ResetPassword",
            "Account",
            new { email = user.Email, token });
        
        EmailMessage message = new("Сброс пароля", "ResetPassword", [user.Email]);
        var isMessageSent = await _emailService.SendPasswordResetLinkAsync(message, resetPasswordLink!, cancellationToken);
        
        return Result.OkIf(isMessageSent, "Fail to sent reset password link");
    }
}