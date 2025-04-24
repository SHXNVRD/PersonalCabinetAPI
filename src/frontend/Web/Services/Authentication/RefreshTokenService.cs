using System.Security.Claims;
using FluentResults;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Web.Services.Authentication;

public class RefreshTokenService
{
    private readonly AuthenticationService _authService;
    private readonly JwtAuthStateProvider _authStateProvider;
    private readonly RefreshTokenOptions _options;

    public RefreshTokenService(
        AuthenticationService authService, 
        JwtAuthStateProvider authStateProvider,
        IOptions<RefreshTokenOptions> options)
    {
        _authService = authService;
        _authStateProvider = authStateProvider;
        _options = options.Value;
    }

    public async Task<Result> TryRefreshAsync()
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        var exp = user.FindFirst(c => c.Type.Equals(JwtRegisteredClaimNames.Exp));
        if (exp is null)
            return Result.Fail("Не удалось извлечь время действия токена доступа.");

        var expAt = DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(exp.Value));
        var diff = expAt - DateTime.UtcNow;

        if (diff.TotalSeconds <= _options.AllowableTokenLifetime)
        {
            var result =  await _authService.RefreshTokenAsync();
            if (result.IsFailed)
                return Result.Fail(result.Errors);
        }

        return Result.Ok();
    }
}