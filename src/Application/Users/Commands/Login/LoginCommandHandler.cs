using Application.DTOs;
using Application.Interfaces;
using Application.Interfaces.Token;
using Application.Users.DTOs;
using Domain.Aggregates.UserAggregate;
using Domain.Shared.Errors;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Commands.Login;

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
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Result.Fail(new Conflict("Wrong password or email"));

        if (user.EmailConfirmed == false)
            return Result.Fail(new Conflict("Email unconfirmed"));

        var passwordCheckedResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!passwordCheckedResult.Succeeded)
            return Result.Fail(new Conflict("Wrong password or email"));

        var accessToken = await _tokenService.GenerateTokenAsync(user);
        var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user);

        return Result.Ok(new AuthResponse(accessToken, refreshToken, 
            _tokenService.TokenType, _tokenService.AccessTokenExpiresInSeconds));
    }
}