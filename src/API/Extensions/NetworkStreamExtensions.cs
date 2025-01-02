using System.Net.Sockets;

namespace API.Extensions;

public static class NetworkStreamExtensions
{
    public static void WaitForData(this NetworkStream stream, TimeSpan? timeout)
    {
        if (timeout == null)
            return;
        
        int originalReadTimeout = stream.ReadTimeout;
        stream.ReadTimeout = (int)timeout.Value.TotalMilliseconds;
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
        stream.WaitForData(startTimeout);
        var writer = new MemoryStream();
        var buffer = new byte[bufferSize];
        
        int bytesRead;
        stream.WaitForData(startTimeout);
            
        while (stream.DataAvailable && (bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
        {
            writer.Write(buffer, 0, bytesRead);
        }

        return writer.ToArray();
    }
}