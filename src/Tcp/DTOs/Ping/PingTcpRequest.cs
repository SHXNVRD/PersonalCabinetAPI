using System.Xml.Serialization;
using Tcp.Abstractions;
using Tcp.DTOs.Base;

namespace Tcp.DTOs.Ping;

[XmlRoot("DP")]
public class PingTcpRequest : ITcpRequest
{
    [XmlElement("M")]
    public M M { get; set; }
    
    [XmlElement("R")]
    public PingR R { get; set; }
}

public class PingR
{
    [XmlElement("ROW")]
    public PingRow Row { get; set; }
}

public class PingRow : RowBase
{
    [XmlAttribute("num_to")]
    public string NumTo { get; set; }

    [XmlAttribute("ver_po")]
    public string VerPo { get; set; }
}
