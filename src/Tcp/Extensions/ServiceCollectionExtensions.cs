using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tcp.Abstractions;
using Tcp.Behaviors;
using Tcp.CommandHandlers;

namespace Tcp.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTcp(this IServiceCollection services, IConfiguration config)
    {
        services
            .Configure<TcpOptions>(config.GetSection(nameof(TcpOptions)))
            .AddScoped<ITcpCommandHandler, PingTcpCommandHandler>()
            .AddScoped<ITcpCommandHandler, CloseShiftTcpCommandHandler>()
            .AddScoped<ITcpCommandHandler, CreatePurchaseTcpCommandHandler>()
            .AddScoped<ITcpPipelineBehavior, TcpRequestsLoggingPipelineBehavior>()
            .AddSingleton<ITcpServer, TcpServer>();

        return services;
    }
}