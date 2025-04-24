using ApiClient;
using ApiClient.Auth;
using ApiClient.Extensions;
using Blazored.LocalStorage;
using FluentResults;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.IdentityModel.Tokens;
using Web.Pages;
using Web.Services.Authentication.DTOs;

namespace Web.Services.Authentication;

public class AuthenticationService
{
    public const string AccessTokenKey = "AccessToken";
    public const string RefreshTokenKey = "RefreshToken";
    
    private readonly JwtAuthStateProvider _authStateProvider;
    private readonly ILocalStorageService _localStorage;
    private readonly GasStationClient _apiClient;

    public AuthenticationService(
        JwtAuthStateProvider authStateProvider, 
        ILocalStorageService localStorage, 
        GasStationClient apiClient)
    {
        _authStateProvider = authStateProvider;
        _localStorage = localStorage;
        _apiClient = apiClient;
    }

    public async Task<Result> RegisterAsync(RegisterRequestDto user)
    {
        var request = new RegisterRequest(user.Email, user.PhoneNumber, user.Password, 
            user.FirstName, user.SecondName, user.Patronymic);
        var response = await _apiClient.AuthClient.RegisterAsync(request);

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

    public async Task<Result<string>> RefreshTokenAsync()
    {
        var accessToken = await _localStorage.GetItemAsStringAsync(AccessTokenKey);
        var refreshToken = await _localStorage.GetItemAsStringAsync(RefreshTokenKey);

        if (string.IsNullOrWhiteSpace(accessToken) || string.IsNullOrWhiteSpace(refreshToken))
            return Result.Fail("Не удалось загрузить токены доступа или токен обновления. Пройдите процедуру авторизации.");

        var request = new RefreshTokenRequest(refreshToken);
        var response = await _apiClient.AuthClient.RefreshToken(request, accessToken);

        if (!response.IsSuccessful)
        {
            var problemDetails = await response.GetErrorAsync();
            var message = problemDetails?.Errors.FirstOrDefault() ?? "Не удалось обновить токен доступа. Пройдите процедуру авторизации.";
            return Result.Fail(message);
        }

        var newAccessToken = response.Content.AccessToken;

        await _localStorage.SetItemAsStringAsync(AccessTokenKey, newAccessToken);
        await _localStorage.SetItemAsStringAsync(RefreshTokenKey, response.Content.RefreshToken);

        return newAccessToken;
    }

    private async Task<Result<AuthResponseDto>> AuthenticateAsync(string password, string? userName, string? email)
    {
        var request = new LoginRequest(password, userName, email);
        var response = await _apiClient.AuthClient.LoginAsync(request);
        
        if (!response.IsSuccessful)
        {
            var problemDetails = await response.GetErrorAsync();
            var message = problemDetails?.Errors.FirstOrDefault() ?? "Не удалось авторизоваться.";
            return Result.Fail(message);
        }

        var content = response.Content;
        if (content is null
            || string.IsNullOrWhiteSpace(content.AccessToken)
            || string.IsNullOrWhiteSpace(content.RefreshToken))
        {
            return Result.Fail("Некорректный овтет сервера");
        }

        await _localStorage.SetItemAsStringAsync(AccessTokenKey, content.AccessToken);
        await _localStorage.SetItemAsStringAsync(RefreshTokenKey, content.RefreshToken);
        
        return new AuthResponseDto(content.AccessToken, content.TokenType, content.ExpiresIn, content.RefreshToken);
    }
}