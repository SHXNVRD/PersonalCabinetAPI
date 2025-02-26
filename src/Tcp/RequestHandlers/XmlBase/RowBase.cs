using System.Xml.Serialization;
using Tcp.RequestHandlers.Ping;

namespace Tcp.RequestHandlers.XmlBase;

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