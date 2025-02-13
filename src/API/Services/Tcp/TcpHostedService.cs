using API.Abstractions;

namespace API.Services.Tcp;

public class TcpHostedService : BackgroundService
{
    private readonly ITcpServer _tcpServer;
    private readonly IHostApplicationLifetime _lifetime;
    
    public TcpHostedService(
        ITcpServer tcpServer, 
        IHostApplicationLifetime lifetime)
    {
        _tcpServer = tcpServer;
        _lifetime = lifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        if (!await WaitForAppStartup(_lifetime, cancellationToken))
            return;
        
        await _tcpServer.StartAsync(cancellationToken);
    }

    static async Task<bool> WaitForAppStartup(IHostApplicationLifetime lifetime, CancellationToken cancellationToken)
    {
        var startedSource = new TaskCompletionSource();
        await using var reg1 = lifetime.ApplicationStarted.Register(() => startedSource.SetResult());
 
        var cancelledSource = new TaskCompletionSource();
        await using var reg2 = cancellationToken.Register(() => cancelledSource.SetResult());
 
        var completedTask = await Task.WhenAny(startedSource.Task, cancelledSource.Task).ConfigureAwait(false);
 
        return completedTask == startedSource.Task;
    }
}