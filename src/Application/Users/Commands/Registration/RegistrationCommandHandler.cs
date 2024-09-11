using Application.DTOs;
using Application.Extensions;
using Application.Interfaces;
using Domain.Models;    
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Users.Commands.Registration
{
    public class RegistrationCommandHandler : IRequestHandler<RegistrationCommand, Result<AuthResponse>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;

        public RegistrationCommandHandler(UserManager<User> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<Result<AuthResponse>> Handle(RegistrationCommand request, CancellationToken cancellationToken)
        {
            var user = request.ToEntity();

            var userResult = await _userManager.CreateAsync(user, request.Password);

            if (!userResult.Succeeded)
            {
                return userResult.ToFluentResult<AuthResponse>();
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "user");    

            if (!roleResult.Succeeded)
            {
                return roleResult.ToFluentResult<AuthResponse>();
            }

            return Result.Ok(new AuthResponse()
            {
                Token = await _tokenService.GenerateTokenAsync(user),
                RefreshToken = await _tokenService.GenerateRefreshTokenAsync(user),
                TokenType = _tokenService.TokenType,
                ExpiresIn = _tokenService.AccessTokenExpiresInSeconds
            });
        }
    }
}