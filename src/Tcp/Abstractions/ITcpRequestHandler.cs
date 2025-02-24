using System.Xml.Linq;
using FluentResults;
using MediatR;

namespace Tcp.Abstractions;

public interface ITcpRequestHandler<in TRequest>
    where TRequest : ITcpRequest
{
    Task<Result<XDocument>> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
}