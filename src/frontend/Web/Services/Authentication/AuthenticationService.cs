using ApiClient.Auth;
using ApiClient.Extensions;
using Blazored.LocalStorage;
using FluentResults;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Web.Pages;
using Web.Services.Authentication.DTOs;

namespace Web.Services.Authentication;

public class AuthenticationService
{
    public const string AccessTokenKey = "AccessToken";
    public const string RefreshTokenKey = "RefreshToken";
    
    private readonly JwtAuthStateProvider _authStateProvider;
    private readonly ILocalStorageService _localStorage;
    private readonly IAuthClient _authClient;

    public AuthenticationService(
        JwtAuthStateProvider authStateProvider, 
        ILocalStorageService localStorage, 
        IAuthClient authClient)
    {
        _authStateProvider = authStateProvider;
        _localStorage = localStorage;
        _authClient = authClient;
    }

    public async Task<Result> RegisterAsync(RegisterRequestDto user)
    {
        var request = new RegisterRequest(user.Email, user.PhoneNumber, user.Password, 
            user.FirstName, user.SecondName, user.Patronymic);
        var response = await _authClient.RegisterAsync(request);

        if (response.IsSuccessful)
            return Result.Ok();

        var problemDetails = await response.GetErrorAsync();
        var message = problemDetails?.Errors.FirstOrDefault() ?? "Ошибка регистрации аккаунта.";
        
        return Result.Fail(message);
    }

    public async Task<Result> LoginAsync(string password, string? userName, string? email)
    {
        var result = await AuthenticateAsync(password, userName, email);
        if (result.IsSuccess)
            await _authStateProvider.NotifyUserAuthenticationAsync();

        return Result.Fail(result.Errors);
    }

    private async Task<Result<AuthResponseDto>> AuthenticateAsync(string password, string? userName, string? email)
    {
        var request = new LoginRequest(password, userName, email);
        var response = await _authClient.LoginAsync(request);

        if (!response.IsSuccessful)
        {
            var problemDetails = await response.GetErrorAsync();
            var message = problemDetails?.Errors.FirstOrDefault() ?? "Не удалось авторизоваться.";
            return Result.Fail(message);
        }

        var content = response.Content;
        if (content is null)
            return Result.Fail("Некорректный овтет сервера");

        await _localStorage.SetItemAsStringAsync(AccessTokenKey, content.AccessToken);
        await _localStorage.SetItemAsStringAsync(RefreshTokenKey, content.RefreshToken);
        return new AuthResponseDto(content.AccessToken, content.TokenType, content.ExpiresIn, content.RefreshToken);
    }
}