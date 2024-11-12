using Application.DTOs;
using Application.Interfaces;
using Application.Interfaces.Token;
using Application.Users.DTOs;
using Domain.Models;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;

        public LoginCommandHandler(SignInManager<User> signInManager, UserManager<User> userManager, ITokenService tokenService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return Result.Fail("Wrong password or email");

            if (user.EmailConfirmed == false)
                return Result.Fail("Email unconfirmed");

            SignInResult passwordCheckedResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            
            if (!passwordCheckedResult.Succeeded)
                return Result.Fail($"Wrong password or email");
            
            return Result.Ok(new AuthResponse
            {
                Token = await _tokenService.GenerateTokenAsync(user),
                RefreshToken = await _tokenService.GenerateRefreshTokenAsync(user),
                TokenType = _tokenService.TokenType,
                ExpiresIn = _tokenService.AccessTokenExpiresInSeconds
            });
        }
    }
}
