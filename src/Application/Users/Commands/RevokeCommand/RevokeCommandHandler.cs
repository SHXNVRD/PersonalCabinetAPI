using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Models;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Users.Commands
{
    public class RevokeCommandHandler : IRequestHandler<RevokeCommand, Result>
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;

        public RevokeCommandHandler(ITokenService tokenService, UserManager<User> userManager)
        {
            _tokenService = tokenService;
            _userManager = userManager;
        }

        public async Task<Result> Handle(RevokeCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.UserEmail);

            if (user == null)
                return Result.Fail("Invalid email");

            IdentityResult revokeResult = await _tokenService.RevokeRefreshTokenAsync(user);

            if (!revokeResult.Succeeded)
                return Result.Fail("Invalid email");

            return Result.Ok();
        }
    }
}