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
            
            var userResult = await _userManager.CreateAsync(user, request.Password);
            
            if (!userResult.Succeeded)
                return userResult.ToFluentResult();

            var roleResult = await _userManager.AddToRoleAsync(user, "user");

            return Result.OkIf(roleResult.Succeeded, "Failed to add role (user)");
        }
    }
}