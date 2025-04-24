using Refit;

namespace ApiClient.Auth;

public interface IAuthClient
{
    [Post("auth/register")]
    public Task<IApiResponse> RegisterAsync([Body] RegisterRequest request);

    [Post("auth/token")]
    public Task<ApiResponse<AuthResponse>> LoginAsync([Body] LoginRequest request);

    [Post("auth/refresh")]
    public Task<ApiResponse<AuthResponse>> RefreshToken([Body] RefreshTokenRequest request, [Authorize] string token);
    [Post("email/confirmation-link")]
    public Task<IApiResponse> ConfirmEmailAsync([Body] SendEmailConfirmationLinkRequest request);
}