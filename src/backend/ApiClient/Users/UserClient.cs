namespace ApiClient.Users;

public class UserClient : ApiClientExecutor
{
    private const string BaseUri = "/users";
    
    private readonly GasStationClient _client;
    
    public UserClient(GasStationClient client)
        : base(client)
    {
        _client = client;
    }

    public async Task<ApiClientResponse<GetUsersResponse>> GetAllAsync()
    {
        return await GetAsync<GetUsersResponse>($"{BaseUri}");
    }
}