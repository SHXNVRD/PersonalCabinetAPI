using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace Web.Services.Authentication;

public class JwtAuthStateProvider : AuthenticationStateProvider
{
    private const string AuthenticationType = "jwt";
    
    private ClaimsPrincipal _cachedUser = new();
    private readonly AuthenticationState _anonymous = new(new ClaimsPrincipal());
    private readonly TimeSpan _userCacheRefreshInterval = TimeSpan.FromSeconds(60.0);
    private DateTimeOffset _userLastCheck = DateTimeOffset.FromUnixTimeSeconds(0L);
    
    private readonly ILocalStorageService _localStorage;
    
    public JwtAuthStateProvider(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }
    
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = await GetUserAsync(true);
        return new AuthenticationState(user);
    }
    
    public async Task NotifyUserAuthenticationAsync()
    {
        var user = await GetUserAsync();
        var state = new AuthenticationState(user);
        NotifyAuthenticationStateChanged(Task.FromResult(state));
    }
    
    private async Task<ClaimsPrincipal> GetUserAsync(bool useCache = false)
    {
        var now = DateTimeOffset.Now;

        if (useCache && now < _userLastCheck + _userCacheRefreshInterval)
            return _cachedUser;

        var token = await _localStorage.GetItemAsync<string>(AuthenticationService.AccessTokenKey);

        if (string.IsNullOrWhiteSpace(token))
        {
            _cachedUser = _anonymous.User;
            _userLastCheck = now;
            return _cachedUser;
        }

        if (!TryGetPrincipal(token, out var user))
        {
            await _localStorage.RemoveItemsAsync([AuthenticationService.AccessTokenKey, AuthenticationService.RefreshTokenKey]);
            user = _anonymous.User;
        }

        _cachedUser = user;
        _userLastCheck = now;
        return _cachedUser;
    }

    private bool TryGetPrincipal(string token, out ClaimsPrincipal claimsPrincipal)
    {
        claimsPrincipal = new ClaimsPrincipal();
        
        var tokenHandler = new JwtSecurityTokenHandler();
        if (!tokenHandler.CanReadToken(token))
            return false;
        
        var jwtSecurityToken = tokenHandler.ReadJwtToken(token);
        var claimsIdentity = new ClaimsIdentity(jwtSecurityToken.Claims, AuthenticationType);
        claimsPrincipal.AddIdentity(claimsIdentity);
        
        return true;
    }
}