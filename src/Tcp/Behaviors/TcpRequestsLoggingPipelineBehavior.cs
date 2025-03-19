using System.Diagnostics;
using System.Text;
using System.Xml.Linq;
using FluentResults;
using Microsoft.Extensions.Logging;
using Tcp.Abstractions;
using Tcp.Extensions;

namespace Tcp.Behaviors;

public class TcpRequestsLoggingPipelineBehavior<TRequest>
    : ITcpPipelineBehavior<TRequest> where TRequest : ITcpRequest
{
    private readonly ILogger<TcpRequestsLoggingPipelineBehavior<TRequest>> _logger;
    private readonly Stopwatch _stopwatch = new();

    public TcpRequestsLoggingPipelineBehavior(ILogger<TcpRequestsLoggingPipelineBehavior<TRequest>> logger)
    {
        _logger = logger;
    }

    public async Task<Result<XDocument>> HandleAsync(
        TRequest request,
        Func<TRequest, CancellationToken, Task<Result<XDocument>>> next,
        CancellationToken cancellationToken
    )
    {
        var requestName = request.GetType().Name;
        
        _logger.LogInformation("Starting tcp request {RequestName}", requestName);
        
        _stopwatch.Start();
        var result = await next(request, cancellationToken);
        _stopwatch.Stop();

        if (result.IsSuccess)
        {
            _logger.LogInformation("Completed tcp request in {ExecutingTime} ms",
                _stopwatch.ElapsedMilliseconds);
        }
        else
        {
            _logger.LogError("Tcp request failure {@Errors} in {ExecutingTime} ms,",
                result.Errors,
                _stopwatch.ElapsedMilliseconds);
        }

        var response = result.ValueOrDefault;
        if (response is not null)
        {
            var buffer = await response.ToByteArrayAsync(Encoding.UTF8, cancellationToken);
            _logger.LogDebug("{Response}", Encoding.UTF8.GetString(buffer));
        }
        
        _stopwatch.Reset();
        return result;
    }
}