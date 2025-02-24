using System.Xml.Serialization;
using Tcp.DTOs.Ping;

namespace Tcp.DTOs.Base;

[XmlInclude(typeof(PingRow))]
public class RowBase
{
    [XmlAttribute("cmdtype")]
    public int CmdType { get; set; }
    
    [XmlAttribute("dt")]
    public string Date { get; set; }
    
    [XmlAttribute("lastcheck")]
    public string LastCheck { get; set; }
}