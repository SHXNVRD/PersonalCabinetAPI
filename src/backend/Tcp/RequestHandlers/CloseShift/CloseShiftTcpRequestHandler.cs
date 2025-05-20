using System.Xml.Linq;
using FluentResults;
using Microsoft.Extensions.Options;
using Tcp.Abstractions;
using Tcp.Helpers.CheckBuilder;
using Tcp.Helpers.TerminalResponseBuilder;

namespace Tcp.RequestHandlers.CloseShift;

public class CloseShiftTcpRequestHandler : ITcpRequestHandler<CloseShiftTcpRequest>
{
    private readonly TcpOptions _tcpOptions;

    public CloseShiftTcpRequestHandler(IOptions<TcpOptions> tcpOptions)
    {
        _tcpOptions = tcpOptions.Value;
    }
    
    public Task<Result<XDocument>> HandleAsync(CloseShiftTcpRequest request, CancellationToken cancellationToken = default)
    {
        var settings = new TerminalResponseBuilderSettings(_tcpOptions.Host, _tcpOptions.Port.ToString());
        var responseBuilder = new TerminalResponseBuilder(settings);
        var checkBuilder = new CheckBuilder();

        var check = checkBuilder
            .AddLine("**** Закрытие смены ****")
            .AddLine("****** Svoy.Club ******")
            .Build();

        var response = responseBuilder
            .AddResponseCode("0")
            .AddOFlResponse("0")
            .AddRejection("0")
            .AddRejectionCode("0")
            .AddBalance("100")
            .AddCheckId("0")
            .AddCheck(check)
            .Build();

        return Task.FromResult(Result.Ok(response));
    }
}