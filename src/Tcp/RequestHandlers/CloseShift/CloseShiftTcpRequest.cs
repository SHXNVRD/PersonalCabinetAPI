using System.Xml.Serialization;
using Tcp.Abstractions;
using Tcp.RequestHandlers.XmlBase;

namespace Tcp.RequestHandlers.CloseShift;

[XmlRoot("DP")]
public class CloseShiftTcpRequest : ITcpRequest
{
    [XmlElement("M")]
    public M M { get; set; }
    
    [XmlElement("R")]
    public CloseShiftR R { get; set; }
}

public class CloseShiftR
{
    [XmlElement("ROW")]
    public CloseShiftRow Row { get; set; }
}

public class CloseShiftRow : RowBase
{ }