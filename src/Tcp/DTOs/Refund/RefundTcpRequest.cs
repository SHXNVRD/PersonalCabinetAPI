using System.Xml.Serialization;
using Tcp.Abstractions;
using Tcp.DTOs.Base;

namespace Tcp.DTOs.Refund;

[XmlRoot("DP")]
public class RefundTcpRequest : ITcpRequest
{
    [XmlElement("M")]
    public M M { get; set; }
    
    [XmlElement("R")]
    public RefundR R { get; set; }
}

public class RefundR
{
    [XmlElement("ROW")]
    public RefundRow Row { get; set; }
}

public class RefundRow : RowBase
{
    [XmlAttribute("num_to")]
    public string NumTo { get; set; }

    [XmlAttribute("ver_po")]
    public string VerPo { get; set; }

    [XmlAttribute("oper_kod")]
    public string OperKod { get; set; }

    [XmlAttribute("cardno")]
    public string CardNumber { get; set; }

    [XmlAttribute("hash")]
    public string Hash { get; set; }

    [XmlAttribute("usluga")]
    public int ProductId { get; set; }

    [XmlAttribute("cena")]
    public string ProductPrice { get; set; }

    [XmlAttribute("kol")]
    public int Quantity { get; set; }

    [XmlAttribute("summa")]
    public string Total { get; set; }

    [XmlAttribute("valuta")]
    public string Сurrency { get; set; }

    [XmlAttribute("pin")]
    public string CardPinCode { get; set; }
}
