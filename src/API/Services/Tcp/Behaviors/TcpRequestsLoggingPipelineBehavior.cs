using System.Diagnostics;
using System.Text;
using System.Xml.Linq;
using API.Extensions;
using Application.Tcp;

namespace API.Services.Tcp.Behaviors;

public class TcpRequestsLoggingPipelineBehavior : ITcpPipelineBehavior
{
    private readonly ILogger<TcpRequestsLoggingPipelineBehavior> _logger;
    private readonly Stopwatch _stopwatch = new();

    public TcpRequestsLoggingPipelineBehavior(ILogger<TcpRequestsLoggingPipelineBehavior> logger)
    {
        _logger = logger;
    }

    public async Task<XDocument> HandleAsync(
        XDocument request,
        Func<XDocument, CancellationToken, Task<XDocument>> next,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInformation("Tcp request with code {RequestCode} starting ", 
            request.Root?.Element("R")?.Element("ROW")?.Attribute("cmdtype")?.Value);
        
        var buffer = await request.ToByteArrayAsync(Encoding.GetEncoding(1251));
        var requestString = Encoding.GetEncoding(1251).GetString(buffer);
        
        _logger.LogInformation("Request: {Request}", requestString);
        
        _stopwatch.Start();
        var response = await next(request, cancellationToken);
        _stopwatch.Stop();
        
        _logger.LogInformation("Tcp request with code {RequestCode} finished in {ExecutingTime} ms", 
            request.Root?.Element("R")?.Element("ROW")?.Attribute("cmdtype")?.Value,
            _stopwatch.ElapsedMilliseconds);
        
        _stopwatch.Reset();
        return response;
    }
}