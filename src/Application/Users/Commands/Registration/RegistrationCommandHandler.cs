using System.Security.Policy;
using Application.DTOs;
using Application.DTOs.Emails;
using Application.Extensions;
using Application.Interfaces;
using Application.Interfaces.Email;
using Application.Interfaces.Token;
using Application.Services;
using Application.Users.DTOs;
using Domain.Models;    
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace Application.Users.Commands.Registration
{
    public class RegistrationCommandHandler : IRequestHandler<RegistrationCommand, Result>
    {
        private readonly AppUserManager _userManager;

        public RegistrationCommandHandler(AppUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result> Handle(RegistrationCommand request, CancellationToken cancellationToken)
        {
            var user = request.ToEntity();

            if (!await _userManager.IsUniqueEmailAsync(request.Email))
                return Result.Fail("User with specified email already exist");

            if (!await _userManager.IsUniquePhoneAsync(request.PhoneNumber))
                return Result.Fail("Specified phone number is already taken");
            
            var userResult = await _userManager.CreateAsync(user, request.Password);
            
            if (!userResult.Succeeded)
                return userResult.ToFluentResult();

            var roleResult = await _userManager.AddToRoleAsync(user, "user");

            return Result.OkIf(roleResult.Succeeded, "Failed to add role (user)");
        }
    }
}