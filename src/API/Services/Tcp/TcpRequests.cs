using System.Runtime.Serialization;

namespace API.Services.Tcp;

public static class TcpRequests
{
    public const string Ping = "2000";
    public const string CloseShift = "1006";
    public const string CreatePurchase = "1000";
    public const string Refund = "1001";
}