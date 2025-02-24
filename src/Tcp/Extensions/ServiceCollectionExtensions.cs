using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tcp.Abstractions;
using Tcp.Behaviors;
using Tcp.DTOs.CloseShift;
using Tcp.DTOs.CreatePurchase;
using Tcp.DTOs.Ping;
using Tcp.RequestHandlers;

namespace Tcp.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTcp(this IServiceCollection services, IConfiguration config)
    {
        services
            .Configure<TcpOptions>(config.GetSection(nameof(TcpOptions)))
            .AddScoped<ITcpRequestHandler<PingTcpRequest>, PingTcpRequestHandler>()
            .AddScoped<ITcpRequestHandler<CloseShiftTcpRequest>, CloseShiftTcpRequestHandler>()
            .AddScoped<ITcpRequestHandler<CreatePurchaseTcpRequest>, CreatePurchaseTcpRequestHandler>()
            .AddScoped(typeof(ITcpPipelineBehavior<>), typeof(TcpRequestsLoggingPipelineBehavior<>))
            .AddSingleton<ITcpServer, TcpServer>();

        return services;
    }
}