using Refit;

namespace ApiClient.Auth;

public interface IAuthClient
{
    [Post("auth/register")]
    public Task<IApiResponse> RegisterAsync([Body] RegisterRequest request);

    [Post("auth/token")]
    public Task<ApiResponse<LoginResponse>> LoginAsync([Body] LoginRequest request);

    [Post("email/confirmation-link")]
    public Task<IApiResponse> ConfirmEmailAsync([Body] SendEmailConfirmationLinkRequest request);
}