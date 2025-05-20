using System.Net.Sockets;

namespace Tcp.Extensions;

public static class NetworkStreamExtensions
{
    public static void WaitForData(this NetworkStream stream, TimeSpan? timeout)
    {
        if (timeout == null)
            return;
        
        var originalReadTimeout = stream.ReadTimeout;
        stream.ReadTimeout = (int)timeout.Value.TotalMilliseconds;
        // Не читаем, но ждём поступления данных в течении timeout
        _ = stream.Read(Array.Empty<byte>(), 0, 0);
        stream.ReadTimeout = originalReadTimeout;
    }
    
    public static byte[] ReadUntilTimeout(
        this NetworkStream stream, 
        TimeSpan? startTimeout = null,
        TimeSpan? readTimeout = null,
        int bufferSize = 8192
    )
    {
        stream.ReadTimeout = (int?)readTimeout?.TotalMilliseconds ?? stream.ReadTimeout;
        // если в течение стартового таймаута данные не поступят, будет выброшен SocketException
        stream.WaitForData(startTimeout);
        using var writer = new MemoryStream();
        var buffer = new byte[bufferSize];
        int bytesRead;
        
        while (stream.DataAvailable && (bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
        {
            writer.Write(buffer, 0, bytesRead);
        }

        return writer.ToArray();
    }
}