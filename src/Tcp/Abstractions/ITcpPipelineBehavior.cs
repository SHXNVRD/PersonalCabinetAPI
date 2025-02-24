using System.Xml.Linq;
using FluentResults;

namespace Tcp.Abstractions;

public interface ITcpPipelineBehavior<TRequest>
    where TRequest : notnull
{
    Task<Result<XDocument>> HandleAsync(
        TRequest request,
        Func<TRequest, CancellationToken, Task<Result<XDocument>>> next,
        CancellationToken cancellationToken = default
    );
}