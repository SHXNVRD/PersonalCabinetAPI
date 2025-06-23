using System.Xml.Linq;
using FluentResults;
using Microsoft.Extensions.Options;
using Tcp.Abstractions;
using Tcp.Helpers.TerminalResponseBuilder;

namespace Tcp.RequestHandlers.Ping;

public class PingTcpRequestHandler: ITcpRequestHandler<PingTcpRequest>
{
    private readonly TcpOptions _tcpOptions;
    
    public PingTcpRequestHandler(IOptions<TcpOptions> tcpOptions)
    {
        _tcpOptions = tcpOptions.Value;
    }
    public Task<Result<XDocument>> HandleAsync(PingTcpRequest request, CancellationToken cancellationToken = default)
    {
        var settings = new TerminalResponseBuilderSettings(_tcpOptions.Host, _tcpOptions.Port.ToString());
        var responseBuilder = new TerminalResponseBuilder(settings);
        
        var response = responseBuilder
            .AddResponseCode("0")
            .AddTestMessage("Тестовый чек#13;#10; Строка 2 #13;#10;")
            .AddPingTime("60")
            .AddGetSettings("0")
            .AddToPing("2")
            .AddToCmd("30")
            .AddQ("0")
            .AddFirstHost()
            .AddSecondHost()
            .AddFirstPort()
            .AddSecondPort()
            .AddFirstPortE()
            .AddSecondPortE()
            .Build();

        return Task.FromResult(Result.Ok(response));
    }
}