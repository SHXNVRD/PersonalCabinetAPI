using Application.Interfaces;
using Application.Interfaces.Token;
using Domain.Models;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Users.Commands.RevokeRefreshToken
{
    public class RevokeRefreshTokenCommandHandler : IRequestHandler<RevokeRefreshTokenCommand, Result>
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;

        public RevokeRefreshTokenCommandHandler(ITokenService tokenService, UserManager<User> userManager)
        {
            _tokenService = tokenService;
            _userManager = userManager;
        }

        public async Task<Result> Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user == null)
                return Result.Fail("User with specified id not found");

            IdentityResult revokeResult = await _tokenService.RevokeRefreshTokenAsync(user);

            if (!revokeResult.Succeeded)
                return Result.Fail("Failed to revoke the refresh token");

            return Result.Ok();
        }
    }
}