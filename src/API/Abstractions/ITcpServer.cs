namespace API.Abstractions;

public interface ITcpServer
{
    Task StartAsync(CancellationToken cancellationToken = default);
}