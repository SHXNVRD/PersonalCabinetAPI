using System.Xml.Linq;
using FluentResults;

namespace Tcp.Abstractions;

public interface ITcpCommandHandler
{
    string RequestCode { get; }
    Task<Result<XDocument>> HandleAsync(XDocument request, CancellationToken cancellationToken = default);
}