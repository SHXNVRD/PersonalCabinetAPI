using System.Xml.Linq;
using FluentResults;
using Microsoft.Extensions.Options;
using Tcp.Abstractions;
using Tcp.DTOs.CloseShift;

namespace Tcp.RequestHandlers;

public class CloseShiftTcpRequestHandler : ITcpRequestHandler<CloseShiftTcpRequest>
{
    private readonly TcpOptions _tcpOptions;

    public CloseShiftTcpRequestHandler(IOptions<TcpOptions> tcpOptions)
    {
        _tcpOptions = tcpOptions.Value;
    }
    
    public Task<Result<XDocument>> HandleAsync(CloseShiftTcpRequest request, CancellationToken cancellationToken = default)
    {
        var result = new XDocument(
            new XElement("DP",
                new XElement("M",
                    new XElement("S",
                        new XAttribute("serv1", _tcpOptions.Host),
                        new XAttribute("serv2", _tcpOptions.Host),
                        new XAttribute("portf1", _tcpOptions.Port),
                        new XAttribute("portf2", _tcpOptions.Port),
                        new XAttribute("porte1", _tcpOptions.Port),
                        new XAttribute("porte2", _tcpOptions.Port))),
                new XElement("R",
                    new XElement("ROW",
                        new XAttribute("o_fl_otvet", "0"),
                        new XAttribute("kod_check", "0"),
                        new XAttribute("otkaz", "0"),
                        new XAttribute("balance", "100"),
                        new XAttribute("kod_otkaz", "0"),
                        new XAttribute("kod_otvet_xml", "0"),
                        new XAttribute("checksrc", "**** Закрытие смены ****&#13;&#10;****** Svoy.Club ******")))));

        return Task.FromResult(Result.Ok(result));
    }
}