using Blazored.LocalStorage;

namespace Web.Services.Authentication;

public class RefreshTokenHandler : DelegatingHandler
{
    private readonly RefreshTokenService _refreshTokenService;
    private readonly ILocalStorageService _localStorage;

    public RefreshTokenHandler(RefreshTokenService refreshTokenService, ILocalStorageService localStorage)
    {
        _refreshTokenService = refreshTokenService;
        _localStorage = localStorage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var requestPath = request.RequestUri?.AbsolutePath;

        if (requestPath is not null
            && (requestPath.Contains("token", StringComparison.InvariantCultureIgnoreCase)
                || requestPath.Contains("register", StringComparison.InvariantCultureIgnoreCase)
                || requestPath.Contains("refresh", StringComparison.InvariantCultureIgnoreCase)))
        {
            return await base.SendAsync(request, cancellationToken);
        }

        var result = await _refreshTokenService.TryRefreshAsync();
        if (result.IsFailed)
            return await base.SendAsync(request, cancellationToken);

        var accessToken = await _localStorage.GetItemAsStringAsync(AuthenticationService.AccessTokenKey, cancellationToken);
        if (!string.IsNullOrWhiteSpace(accessToken))
            request.Headers.Authorization = new("Bearer", accessToken); 
        
        return await base.SendAsync(request, cancellationToken);
    }
}