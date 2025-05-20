using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tcp.Abstractions;
using Tcp.Behaviors;
using Tcp.RequestHandlers.CloseShift;
using Tcp.RequestHandlers.CreatePurchase;
using Tcp.RequestHandlers.Ping;
using Tcp.RequestHandlers.Refund;

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
            .AddScoped<ITcpRequestHandler<RefundTcpRequest>, RefundTcpRequestHandler>()
            .AddScoped(typeof(ITcpPipelineBehavior<>), typeof(TcpRequestsLoggingPipelineBehavior<>))
            .AddSingleton<ITcpServer, TcpServer>();

        return services;
    }
}