using System.Xml.Serialization;

namespace Tcp.RequestHandlers.XmlBase;

public class M
{
    [XmlElement("S")]
    public S S { get; set; }
}