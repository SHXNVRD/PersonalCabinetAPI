using System.Xml.Linq;
using FluentResults;

namespace Application.Tcp;

public interface ITcpCommandHandler
{
    string RequestCode { get; }
    Task<Result<XDocument>> HandleAsync(XDocument request, CancellationToken cancellationToken = default);
}