using System.Xml.Linq;

namespace Application.Tcp;

public interface ITcpCommandHandler
{
    string RequestCode { get; }
    Task<XDocument> HandleAsync(XDocument request, CancellationToken cancellationToken = default);
}