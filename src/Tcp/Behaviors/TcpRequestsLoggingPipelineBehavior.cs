using System.Diagnostics;
using System.Xml.Linq;
using FluentResults;
using Microsoft.Extensions.Logging;
using Tcp.Abstractions;

namespace Tcp.Behaviors;

public class TcpRequestsLoggingPipelineBehavior : ITcpPipelineBehavior
{
    private readonly ILogger<TcpRequestsLoggingPipelineBehavior> _logger;
    private readonly Stopwatch _stopwatch = new();

    public TcpRequestsLoggingPipelineBehavior(ILogger<TcpRequestsLoggingPipelineBehavior> logger)
    {
        _logger = logger;
    }

    public async Task<Result<XDocument>> HandleAsync(
        XDocument request,
        Func<XDocument, CancellationToken, Task<Result<XDocument>>> next,
        CancellationToken cancellationToken
    )
    {
        var requestCode = request
            .Descendants("ROW")
            .First()
            .Attribute("cmdtype")
            !.Value;

        _logger.LogInformation("Starting Tcp request with code {RequestCode}", requestCode);
        
        _stopwatch.Start();
        var result = await next(request, cancellationToken);
        _stopwatch.Stop();

        if (result.IsSuccess)
        {
            _logger.LogInformation("Tcp request with code {RequestCode} successfully completed in {ExecutingTime} ms",
                requestCode,
                _stopwatch.ElapsedMilliseconds);
        }
        else
        {
            _logger.LogError("Tcp request with code {RequestCode} failure {@Errors} in {ExecutingTime} ms,",
                requestCode,
                result.Errors,
                _stopwatch.ElapsedMilliseconds);
        }
        
        _stopwatch.Reset();
        return result;
    }
}