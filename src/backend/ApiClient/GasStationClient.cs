using ApiClient.Auth;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ApiClient;

public class GasStationClient
{
    public HttpClient HttpClient { get; }
    public Action<string> Log { get; }
    public AuthClient AuthClient { get; }

    public GasStationClient(HttpClient client, Action<string> log)
    {
        HttpClient = client;
        Log = log;
        AuthClient = new AuthClient(this);
    }
    
    [ActivatorUtilitiesConstructor]
    public GasStationClient(HttpClient client, ILogger<GasStationClient> log) :
        this(client, p => log.LogInformation("API: {Message}", p))
    { }
}