using System.Xml.Serialization;

namespace Tcp.RequestHandlers.XmlBase;

public class RowBase
{
    [XmlAttribute("cmdtype")]
    public int CmdType { get; set; }
    
    [XmlAttribute("dt")]
    public string Date { get; set; }
    
    [XmlAttribute("lastcheck")]
    public string LastCheck { get; set; }
}