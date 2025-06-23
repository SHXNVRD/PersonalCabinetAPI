using System.Net;

namespace Web.Services.Authentication;

public class UnauthorizedHandler : DelegatingHandler
{
    private readonly AuthenticationService _authService;

    public UnauthorizedHandler(AuthenticationService authService)
    {
        _authService = authService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
            await _authService.LogoutAsync();

        return response;
    }
}