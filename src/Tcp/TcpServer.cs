using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tcp.Abstractions;
using Tcp.Extensions;
using Tcp.RequestHandlers.CloseShift;
using Tcp.RequestHandlers.CreatePurchase;
using Tcp.RequestHandlers.Ping;
using Tcp.RequestHandlers.Refund;

namespace Tcp;

public class TcpServer : ITcpServer
{
    private readonly TcpListener _tcpListener;
    private readonly ILogger<TcpServer> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly TcpOptions _options;

    private readonly TimeSpan _startTimeout;
    private readonly TimeSpan _readTimeout;
    
    public TcpServer(
        ILogger<TcpServer> logger, 
        IOptions<TcpOptions> options,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
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
            _ = Task.Run(() => HandleClientAsync(tcpClient, cancellationToken), cancellationToken);
        }
    }
    
    private async Task HandleClientAsync(TcpClient tcpClient, CancellationToken cancellationToken = default)
    {
        var remoteHost = ((IPEndPoint)tcpClient.Client.RemoteEndPoint!).Address.ToString();
        var remotePort = ((IPEndPoint)tcpClient.Client.RemoteEndPoint!).Port.ToString();
        _logger.LogInformation(
            "Remote device with address: {ClientAddress} has connected", $"{remoteHost}:{remotePort}");

        try
        {
            var stream = tcpClient.GetStream();
            var buffer = stream.ReadUntilTimeout(_startTimeout, _readTimeout);
            var cmdType = Encoding
                .GetEncoding(1251)
                .GetString(buffer)
                .GetBetweenOrDefault("cmdtype=\"", "\"");

            if (string.IsNullOrWhiteSpace(cmdType))
            {
                _logger.LogError("Received xml must contain \"cmdtype\" attribute");
                return;
            }
            
            _logger.LogDebug("{Request}", 
                Encoding
                .GetEncoding(1251)
                .GetString(buffer));

            var request = ParseRequest(cmdType, buffer);

            if (request == null)
            {
                _logger.LogError("Failed to deserialize received xml");
                return;
            }
            
            using var scope = _serviceScopeFactory.CreateScope();
            var scopedProvider = scope.ServiceProvider;
            
            var requestType = request!.GetType();
            var pipelineType = typeof(TcpHandlerPipeline<>).MakeGenericType(requestType);
            var pipeline = Activator.CreateInstance(
                pipelineType,
                scopedProvider.GetService(typeof(ITcpRequestHandler<>).MakeGenericType(requestType)),
                scopedProvider.GetServices(typeof(ITcpPipelineBehavior<>).MakeGenericType(requestType))
            );
            var executeMethod = pipelineType.GetMethod("ExecuteAsync");
            var response = await (Task<Result<XDocument>>)executeMethod.Invoke(pipeline, new object[] { request, cancellationToken });
            
            if (response.IsFailed)
                return;
                
            await SendAsync(stream, response.Value);
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
        var buffer = await response.ToByteArrayAsync(Encoding.UTF8, cancellationToken);
        await writer.WriteAsync(buffer, cancellationToken);
        await writer.FlushAsync(cancellationToken);
    }
    
    private static ITcpRequest? ParseRequest(string cmdType, byte[] buffer)
        => cmdType switch
        {
            TcpRequests.Ping => Deserialize<PingTcpRequest>(buffer),
            TcpRequests.CreatePurchase => Deserialize<CreatePurchaseTcpRequest>(buffer),
            TcpRequests.Refund => Deserialize<RefundTcpRequest>(buffer),
            TcpRequests.CloseShift => Deserialize<CloseShiftTcpRequest>(buffer),
            _ => throw new ArgumentOutOfRangeException(nameof(cmdType))
        };

    private static T? Deserialize<T>(byte[] buffer)
    {
        using var stream = new MemoryStream(buffer);
        var serializer = new XmlSerializer(typeof(T));
        if (serializer.Deserialize(stream) is T deserialized)
            return deserialized;

        return default;
    }
}