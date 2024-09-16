using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Models;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Users.Commands.RevokeToken
{
    public class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand, Result>
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;

        public RevokeTokenCommandHandler(ITokenService tokenService, UserManager<User> userManager)
        {
            _tokenService = tokenService;
            _userManager = userManager;
        }

        public async Task<Result> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
        {
            if (!_tokenService.TryGetPrincipal(request.AccessToken, out ClaimsPrincipal claims))
                return Result.Fail("Inavlid token");
            
            var userId = claims.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Result.Fail("Token does not contain user id");

            var user =  await _userManager.FindByIdAsync(userId);

            if (user == null)
                return Result.Fail("User with specified token not found");

            IdentityResult revokeResult = await _tokenService.RevokeRefreshTokenAsync(user);

            if (!revokeResult.Succeeded)
                return Result.Fail("Failed to refresh token");

            return Result.Ok();
        }
    }
}