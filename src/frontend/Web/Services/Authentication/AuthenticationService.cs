using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ApiClient;
using ApiClient.Auth;
using ApiClient.Extensions;
using Blazored.LocalStorage;
using FluentResults;
using Microsoft.AspNetCore.Components.Authorization;
using Web.Services.Authentication.DTOs;

namespace Web.Services.Authentication;

public class AuthenticationService
{
    public const string AccessTokenKey = "AccessToken";
    public const string RefreshTokenKey = "RefreshToken";
    
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _client;

    public AuthenticationService(
        AuthenticationStateProvider authStateProvider, 
        ILocalStorageService localStorage, 
        HttpClient client)
    {
        _authStateProvider = authStateProvider;
        _localStorage = localStorage;
        _client = client;
    }

    public async Task<Result> LoginAsync(string password, string login)
    {
        var result = await AuthenticateAsync(password, login);
        if (result.IsFailed)
            return Result.Fail(result.Errors);

        await ((JwtAuthStateProvider)_authStateProvider).NotifyUserAuthenticationAsync();
        return Result.Ok();
    }

    public async Task<Result<string>> RefreshTokenAsync()
    {
        var accessToken = await _localStorage.GetItemAsStringAsync(AccessTokenKey);
        var refreshToken = await _localStorage.GetItemAsStringAsync(RefreshTokenKey);

        if (string.IsNullOrWhiteSpace(accessToken) || string.IsNullOrWhiteSpace(refreshToken))
            return Result.Fail("Не удалось загрузить токены доступа или токен обновления. Пройдите процедуру авторизации.");

        var request = new RefreshTokenRequest(refreshToken);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await _client.PostAsync("auth/refresh", JsonContent.Create(request));
        
        if (!response.IsSuccessStatusCode)
        {
            var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(await response.Content.ReadAsStringAsync());
            var message = problemDetails?.Errors.FirstOrDefault() ?? "Не удалось обновить токен доступа. Пройдите процедуру авторизации.";
            return Result.Fail(message);
        }

        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();

        if (result is null)
            return Result.Fail("Некорректный ответ сервера");

        await _localStorage.SetItemAsStringAsync(AccessTokenKey, result.AccessToken);
        await _localStorage.SetItemAsStringAsync(RefreshTokenKey, result.RefreshToken);

        return result.AccessToken;
    }

    private async Task<Result<AuthResponseDto>> AuthenticateAsync(string password, string login)
    {
        var request = new LoginRequest(password, login);
        var response = await _client.PostAsync(new Uri("auth/token", UriKind.Relative), JsonContent.Create(request));
        _client.DefaultRequestHeaders.Clear();
        
        if (!response.IsSuccessStatusCode)
        {
            var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(await response.Content.ReadAsStringAsync());
            var message = problemDetails?.Errors.FirstOrDefault() ?? "Не удалось авторизоваться.";
            return Result.Fail(message);
        }

        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        
        if (result is null
            || string.IsNullOrWhiteSpace(result.AccessToken)
            || string.IsNullOrWhiteSpace(result.RefreshToken))
        {
            return Result.Fail("Некорректный овтет сервера");
        }

        await _localStorage.SetItemAsStringAsync(AccessTokenKey, result.AccessToken);
        await _localStorage.SetItemAsStringAsync(RefreshTokenKey, result.RefreshToken);
        
        return new AuthResponseDto(result.AccessToken, result.TokenType, result.ExpiresIn, result.RefreshToken);
    }
}