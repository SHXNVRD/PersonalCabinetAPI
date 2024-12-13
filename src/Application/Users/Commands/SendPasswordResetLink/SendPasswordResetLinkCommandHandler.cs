using Application.DTOs.Emails;
using Application.Interfaces;
using Application.Interfaces.Email;
using Application.Services;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Application.Users.Commands.SendPasswordResetLink;

public class SendPasswordResetLinkCommandHandler : IRequestHandler<SendPasswordResetLinkCommand, Result>
{
    private readonly IEmailService _emailService;
    private readonly ILinkService _linkService;
    private readonly AppUserManager _userManager;

    public SendPasswordResetLinkCommandHandler(
        AppUserManager userManager,
        IEmailService emailService,
        ILinkService linkService)
    {
        _userManager = userManager;
        _emailService = emailService;
        _linkService = linkService;
    }

    public async Task<Result> Handle(SendPasswordResetLinkCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Result.Fail("User with specified email not found");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var resetPasswordLink = _linkService.GetUriByAction(
            "ResetPassword",
            "Account",
            new { email = user.Email, token });
        
        EmailMessage message = new("Сброс пароля", "ResetPassword", [user.Email]);
        var isMessageSent = await _emailService.SendPasswordResetLinkAsync(message, resetPasswordLink!, cancellationToken);
        
        return Result.OkIf(isMessageSent, "Fail to sent reset password link");
    }
}