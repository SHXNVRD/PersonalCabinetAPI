using ApiClient.Auth;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace ApiClient.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiClients(this IServiceCollection services)
    {
        services
            .AddRefitClient<IAuthClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://localhost:8082/api/auth"));

        return services;
    }
}