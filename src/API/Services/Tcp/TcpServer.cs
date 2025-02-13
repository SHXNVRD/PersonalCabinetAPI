using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using API.Abstractions;
using API.Extensions;
using MediatR;
using Microsoft.Extensions.Options;

namespace API.Services.Tcp;

public class TcpServer : ITcpServer
{
    private readonly TcpListener _tcpListener;
    private readonly ILogger<TcpServer> _logger;
    private readonly TcpOptions _options;

    private readonly TimeSpan _startTimeout;
    private readonly TimeSpan _readTimeout;

    private const string CmdPing = "2000";
    public TcpServer(
        ILogger<TcpServer> logger, 
        IOptions<TcpOptions> options)
    {
        _logger = logger;
        _options = options.Value;
        _tcpListener = new TcpListener(IPAddress.Parse(_options.Host), _options.Port);
        _startTimeout = TimeSpan.FromMilliseconds(_options.StartTimeout);
        _readTimeout = TimeSpan.FromMilliseconds(_options.ReadTimeout);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _tcpListener.Start();
        _logger.LogInformation("Tcp server listening on: {Address}", $"{_options.Host}:{_options.Port}");

        while (!cancellationToken.IsCancellationRequested)
        {
            var tcpClient = await _tcpListener.AcceptTcpClientAsync(cancellationToken);
            _ = Task.Run(() => MeasureTimeAsync(() => HandleClientAsync(tcpClient)), cancellationToken);
        }
    }

    private async Task MeasureTimeAsync(Func<Task> func)
    {
        var watch = Stopwatch.StartNew();
        await func.Invoke();
        watch.Stop();
        _logger.LogInformation("Tcp request executed in {Time} ms", watch.ElapsedMilliseconds);
    }
    
    private async Task HandleClientAsync(TcpClient tcpClient)
    {
        var remoteHost = ((IPEndPoint)tcpClient.Client.RemoteEndPoint!).Address.ToString();
        var remotePort = ((IPEndPoint)tcpClient.Client.RemoteEndPoint!).Port.ToString();
        _logger.LogInformation(
            "Remote device with address: {ClientAddress} has connected", 
            $"{remoteHost}:{remotePort}");
        
        try
        {
            var stream = tcpClient.GetStream();
            var xml = await stream
                .ReadUntilTimeout(_startTimeout, _readTimeout)
                .ToXDocumentAsync();
            
            var cmdType = xml
                .Element("DP")
                ?.Element("R")
                ?.Element("ROW")
                ?.Attribute("cmdtype");

            if (cmdType == null)
            {
                _logger.LogError("Received xml must contain \"cmdtype\" attribute");
                return;
            }

            switch (cmdType.Value)
            {
                case CmdPing:
                    await SendPingAsync(stream);
                    break;
            }
        }
        catch (IOException ioe) when (ioe.InnerException is SocketException soe)
        {
            if (soe.SocketErrorCode != SocketError.TimedOut)
                _logger.LogError(ioe, "Exception has occurred while executing the tcp request: {Message}", ioe.Message);
            _logger.LogError("Timeout, reason: {Message}", soe.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception has occurred while executing the tcp request: {Message}", e.Message);
        }
        finally
        {
            tcpClient.Close();
            _logger.LogInformation(
                "Remote device with address: {ClientAddress} has disconnected",
                $"{remoteHost}:{remotePort}");
        }
    }

    private Task SendPingAsync(NetworkStream writer, CancellationToken cancellationToken = default)
    {
        var response = new XDocument(
            new XElement("DP",
                new XElement("M",
                    new XElement("S",
                        new XAttribute("serv1", _options.Host),
                        new XAttribute("portf1", _options.Port),
                        new XAttribute("porte1", _options.Port),
                        new XAttribute("serv2", _options.Host),
                        new XAttribute("portf2", _options.Port),
                        new XAttribute("porte2", _options.Port))),
                new XElement("R",
                    new XElement("ROW",
                        new XAttribute("kod_otvet_xml", "0"),
                        new XAttribute("test_mes", "Тестовый чек#13;#10; Строка 2 #13;#10;"),
                        new XAttribute("pingtime", "60"), // заменить на реальное время
                        new XAttribute("get_settings", "0"),
                        new XAttribute("to_ping", "2"),
                        new XAttribute("to_cmd", "30"),
                        new XAttribute("q", "0"),
                        new XAttribute("s1", _options.Host),
                        new XAttribute("s2", _options.Host),
                        new XAttribute("p1", _options.Port),
                        new XAttribute("p2", _options.Port),
                        new XAttribute("p1e", _options.Port),
                        new XAttribute("p2e", _options.Port)))));

        return SendAsync(writer, response, cancellationToken);
    }

    private async Task SendAsync(NetworkStream writer, XDocument response, CancellationToken cancellationToken = default)
    {
        var buffer = await response.ToByteArrayAsync(Encoding.GetEncoding(1251), cancellationToken);
        Console.WriteLine(Encoding.GetEncoding(1251).GetString(buffer));
        await writer.WriteAsync(buffer, cancellationToken);
        await writer.FlushAsync(cancellationToken);
    }
}