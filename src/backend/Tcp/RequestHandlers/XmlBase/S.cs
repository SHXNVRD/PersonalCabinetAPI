using System.Xml.Serialization;

namespace Tcp.RequestHandlers.XmlBase;

public class S
{
    [XmlAttribute("numto")]
    public string NumTo { get; set; }

    [XmlAttribute("hash")]
    public string Hash { get; set; }

    [XmlAttribute("Ver")]
    public string Version { get; set; }
}

