using System.Dynamic;
using ApiClient.Auth;
using Refit;

namespace ApiClient;

public class GasStationClient
{
    public HttpClient HttpClient { get; }
    public IAuthClient AuthClient { get; }

    public GasStationClient(HttpClient client)
    {
        HttpClient = client;
        AuthClient = RestService.For<IAuthClient>(client);
    }
}