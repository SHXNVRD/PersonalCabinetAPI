namespace Tcp;

public class TcpOptions
{
    public int StartTimeout { get; set; }
    public int ReadTimeout { get; set; }
    public string Host { get; init; } = null!;
    public int Port { get; init; }
}