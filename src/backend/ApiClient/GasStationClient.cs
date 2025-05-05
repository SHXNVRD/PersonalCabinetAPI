using ApiClient.Users;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ApiClient;

public class GasStationClient
{
    public HttpClient HttpClient { get; }
    public Action<string> Log { get; }
    public UserClient UserClient { get; }

    public GasStationClient(HttpClient client, Action<string> log)
    {
        HttpClient = client;
        Log = log;
        UserClient = new UserClient(this);
    }
    
    [ActivatorUtilitiesConstructor]
    public GasStationClient(HttpClient client, ILogger<GasStationClient> log) :
        this(client, p => log.LogInformation("API: {Message}", p))
    { }
}