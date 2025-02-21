using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using API.Abstractions;
using API.Extensions;
using Application.Tcp;
using MediatR;
using Microsoft.Extensions.Options;

namespace API.Services.Tcp;

public class TcpServer : ITcpServer
{
    private readonly TcpListener _tcpListener;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<TcpServer> _logger;
    private readonly TcpOptions _options;

    private readonly TimeSpan _startTimeout;
    private readonly TimeSpan _readTimeout;
    
    
    public TcpServer(
        ILogger<TcpServer> logger, 
        IOptions<TcpOptions> options,
        IServiceProvider serviceProvider,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _options = options.Value;
        _serviceScopeFactory = serviceScopeFactory;
        
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
            _ = Task.Run(() => HandleClientAsync(tcpClient), cancellationToken);
        }
    }
    
    private async Task HandleClientAsync(TcpClient tcpClient)
    {
        var remoteHost = ((IPEndPoint)tcpClient.Client.RemoteEndPoint!).Address.ToString();
        var remotePort = ((IPEndPoint)tcpClient.Client.RemoteEndPoint!).Port.ToString();
        _logger.LogInformation(
            "Remote device with address: {ClientAddress} has connected", $"{remoteHost}:{remotePort}");
        
        using var scope = _serviceScopeFactory.CreateScope();
        var handlers = scope.ServiceProvider.GetServices<ITcpCommandHandler>();
        var handlerPipelines = handlers.ToDictionary(
            h => h.RequestCode,
            h => new TcpHandlerPipeline(h, scope.ServiceProvider.GetServices<ITcpPipelineBehavior>()));
        
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
                ?.Attribute("cmdtype")
                ?.Value;

            if (string.IsNullOrWhiteSpace(cmdType))
            {
                _logger.LogError("Received xml must contain \"cmdtype\" attribute");
                return;
            }
            
            if (handlerPipelines.TryGetValue(cmdType, out var pipeline))
            {
                var response = await pipeline.ExecuteAsync(xml);

                if (response.IsFailed)
                    return;
                
                await SendAsync(stream, response.Value);
            }
            else
            {
                _logger.LogError("Unsupported request code, attribute cmdtype was {requestCode}", cmdType);
            }
        }
        catch (IOException ioe) when (ioe.InnerException is SocketException soe)
        {
            // Протокол пострен на таймаутах. Истечение таймаута - триггер для закрытия соединения
            // Истечение таймаута означает корректное завершение обмена сообщениями
            if (soe.SocketErrorCode != SocketError.TimedOut)
                _logger.LogError(ioe, "Exception has occurred while executing the tcp request: {Message}", ioe.Message);
            _logger.LogInformation("Connection closed, reason: timeout expired");
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

    private async Task SendAsync(NetworkStream writer, XDocument response, CancellationToken cancellationToken = default)
    {
        var buffer = await response.ToByteArrayAsync(Encoding.GetEncoding(1251), cancellationToken);
        await writer.WriteAsync(buffer, cancellationToken);
        await writer.FlushAsync(cancellationToken);
    }
}