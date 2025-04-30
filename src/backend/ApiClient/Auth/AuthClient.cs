using System.Net.Http.Headers;

namespace ApiClient.Auth;

public class AuthClient : ApiClientExecutor
{
    private readonly GasStationClient _client;

    public AuthClient(GasStationClient client)
        : base(client)
    {
        _client = client;
    }

    private const string BaseUri = "/auth";

    protected override string ApiPrefix => "";

    public Task<ApiClientResponse<AuthResponse>> LoginAsync(LoginRequest request)
    {
        return PostAsync<AuthResponse>($"{BaseUri}/token", request);
    }

    public Task<ApiClientResponse<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request, string accessToken)
    {
        _client.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response =  PostAsync<AuthResponse>($"{BaseUri}/refresh", request);
        _client.HttpClient.DefaultRequestHeaders.Clear();
        return response;
    }
}