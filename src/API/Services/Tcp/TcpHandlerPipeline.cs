using System.Xml.Linq;
using Application.Tcp;

namespace API.Services.Tcp;

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

    public Task<XDocument> ExecuteAsync(
        XDocument request, 
        CancellationToken cancellationToken = default
    )
    {
        Func<XDocument, CancellationToken, Task<XDocument>> pipeline = 
            (req, cToken) => _handler.HandleAsync(req, cToken);
        
        foreach (var behavior in _behaviors.Reverse())
        {
            var next = pipeline;
            pipeline = (req, cToken) => behavior.HandleAsync(req, next, cToken);
        }

        return pipeline(request, cancellationToken);
    }
}