using Application.Extensions;
using Application.Services;
using Domain.Models;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Users.Commands.EmailConfirmation
{
    public class EmailConfirmationRequestHandler : IRequestHandler<EmailConfirmationCommand, Result>
    {
        private readonly AppUserManager _userManager;

        public EmailConfirmationRequestHandler(AppUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result> Handle(EmailConfirmationCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
                return Result.Fail("User with the specified email address was not found");

            var result = await _userManager.ConfirmEmailAsync(user, request.Token);

            return Result.OkIf(result.Succeeded, "Failed to confirmation email");
        }
    }
}