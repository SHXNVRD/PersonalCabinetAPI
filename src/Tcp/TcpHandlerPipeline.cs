using System.Xml.Linq;
using FluentResults;
using Tcp.Abstractions;

namespace Tcp;

public class TcpHandlerPipeline
{
    private readonly IEnumerable<ITcpPipelineBehavior> _behaviors;
    private readonly ITcpCommandHandler _handler;

    public TcpHandlerPipeline(
        ITcpCommandHandler handler,
        IEnumerable<ITcpPipelineBehavior> behaviors)
    {
        _handler = handler;
        _behaviors = behaviors;
    }

    public Task<Result<XDocument>> ExecuteAsync(
        XDocument request, 
        CancellationToken cancellationToken = default
    )
    {
        Func<XDocument, CancellationToken, Task<Result<XDocument>>> pipeline = 
            (req, cToken) => _handler.HandleAsync(req, cToken);
        
        foreach (var behavior in _behaviors.Reverse())
        {
            var next = pipeline;
            pipeline = (req, cToken) => behavior.HandleAsync(req, next, cToken);
        }

        return pipeline(request, cancellationToken);
    }
}