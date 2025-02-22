using System.Xml.Linq;
using FluentResults;

namespace Tcp.Abstractions;

public interface ITcpPipelineBehavior
{
    Task<Result<XDocument>> HandleAsync(
        XDocument request,
        Func<XDocument, CancellationToken, Task<Result<XDocument>>> next,
        CancellationToken cancellationToken = default
    );
}