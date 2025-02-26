using System.Xml.Linq;
using FluentResults;
using Tcp.Abstractions;

namespace Tcp;

public class TcpHandlerPipeline<TRequest>
    where TRequest : ITcpRequest
{
    private readonly IEnumerable<ITcpPipelineBehavior<TRequest>> _behaviors;
    private readonly ITcpRequestHandler<TRequest> _handler;

    public TcpHandlerPipeline(
        ITcpRequestHandler<TRequest> handler,
        IEnumerable<ITcpPipelineBehavior<TRequest>> behaviors)
    {
        _handler = handler;
        _behaviors = behaviors;
    }

    public async Task<Result<XDocument>> ExecuteAsync(
        TRequest command, 
        CancellationToken cancellationToken
    )
    {
        Func<TRequest, CancellationToken, Task<Result<XDocument>>> pipeline = 
            (req, ct) => _handler.HandleAsync(req, ct);
        
        foreach (var behavior in _behaviors.Reverse())
        {
            var next = pipeline;
            pipeline = (req, ct) => behavior.HandleAsync(req, next, ct);
        }

        return await pipeline(command, cancellationToken);
    }
}