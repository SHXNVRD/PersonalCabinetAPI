namespace Tcp;

public record TcpOptions
{
    public int StartTimeout { get; init; }
    public int ReadTimeout { get; init; }
    public string Host { get; init; } = null!;
    public int Port { get; init; }
}