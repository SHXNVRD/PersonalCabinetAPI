using System.Xml.Linq;

namespace Application.Tcp;

public interface ITcpPipelineBehavior
{
    Task<XDocument> HandleAsync(
        XDocument request,
        Func<XDocument, CancellationToken, Task<XDocument>> next,
        CancellationToken cancellationToken = default
    );
}