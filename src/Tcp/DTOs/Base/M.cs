using System.Xml.Serialization;

namespace Tcp.DTOs.Base;

public class M
{
    [XmlElement("S")]
    public S S { get; set; }
}