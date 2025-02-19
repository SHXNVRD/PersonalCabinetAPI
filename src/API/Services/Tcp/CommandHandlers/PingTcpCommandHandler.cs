using System.Xml.Linq;
using Application.Tcp;
using Microsoft.Extensions.Options;

namespace API.Services.Tcp.CommandHandlers;

public class PingTcpCommandHandler : ITcpCommandHandler
{
    private readonly TcpOptions _tcpOptions;
    public string RequestCode => TcpRequests.Ping;

    public PingTcpCommandHandler(IOptions<TcpOptions> tcpOptions)
    {
        _tcpOptions = tcpOptions.Value;
    }
    public Task<XDocument> HandleAsync(XDocument request, CancellationToken cancellationToken = default)
    {
        var response = new XDocument(
            new XElement("DP",
                new XElement("M",
                    new XElement("S",
                        new XAttribute("serv1", _tcpOptions.Host),
                        new XAttribute("portf1", _tcpOptions.Port),
                        new XAttribute("porte1", _tcpOptions.Port),
                        new XAttribute("serv2", _tcpOptions.Host),
                        new XAttribute("portf2", _tcpOptions.Port),
                        new XAttribute("porte2", _tcpOptions.Port))),
                new XElement("R",
                    new XElement("ROW",
                        new XAttribute("kod_otvet_xml", "0"),
                        new XAttribute("test_mes", "Тестовый чек#13;#10; Строка 2 #13;#10;"),
                        new XAttribute("pingtime", "60"), // заменить на реальное время
                        new XAttribute("get_settings", "0"),
                        new XAttribute("to_ping", "2"),
                        new XAttribute("to_cmd", "30"),
                        new XAttribute("q", "0"),
                        new XAttribute("s1", _tcpOptions.Host),
                        new XAttribute("s2", _tcpOptions.Host),
                        new XAttribute("p1", _tcpOptions.Port),
                        new XAttribute("p2", _tcpOptions.Port),
                        new XAttribute("p1e", _tcpOptions.Port),
                        new XAttribute("p2e", _tcpOptions.Port)))));

        return Task.FromResult(response);
    }
}