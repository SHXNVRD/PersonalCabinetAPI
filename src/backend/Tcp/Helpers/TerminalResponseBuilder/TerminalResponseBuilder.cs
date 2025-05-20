using System.Xml.Linq;

namespace Tcp.Helpers.TerminalResponseBuilder;

public class TerminalResponseBuilder : ITerminalResponseBuilder
{
    private readonly Dictionary<string, XAttribute> _attributes = new();
    private XDocument _xDocument;
    
    private readonly string _host;
    private readonly string _port;

    public TerminalResponseBuilder(TerminalResponseBuilderSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        
        _host = settings.Host ?? throw new NullReferenceException("Host cannot be null");
        _port = settings.Port ?? throw new NullReferenceException("Port cannot be null");
        
        Reset();
    }

    public ITerminalResponseBuilder AddResponseCode(string code)
    {
        _attributes["kod_otvet_xml"] = new XAttribute("kod_otvet_xml", code);
        return this;
    }

    public ITerminalResponseBuilder AddOFlResponse(string code)
    {
        _attributes["o_fl_otvet"] = new XAttribute("o_fl_otvet", code);
        return this;
    }

    public ITerminalResponseBuilder AddRejection(string rejection)
    {
        _attributes["otkaz"] = new XAttribute("otkaz", rejection);
        return this;
    }

    public ITerminalResponseBuilder AddRejectionCode(string code)
    {
        _attributes["kod_otkaz"] = new XAttribute("kod_otkaz", code);
        return this;
    }

    public ITerminalResponseBuilder AddBalance(string balance)
    {
        _attributes[balance] = new XAttribute("balance", balance);
        return this;
    }

    public ITerminalResponseBuilder AddCheckId(string id)
    {
        _attributes["kod_check"] = new XAttribute("kod_check", id);
        return this;
    }

    public ITerminalResponseBuilder AddCheck(string check)
    {
        _attributes["check_src"] = new XAttribute("check_src", check);
        return this;
    }

    public ITerminalResponseBuilder AddCardNumber(string number)
    {
        _attributes["cardno"] = new XAttribute("cardno", number);
        return this;
    }

    public ITerminalResponseBuilder AddTestMessage(string message)
    {
        _attributes["test_mes"] = new XAttribute("test_mes", message);
        return this;
    }

    public ITerminalResponseBuilder AddPingTime(string time)
    {
        _attributes["pingtime"] = new XAttribute("pingtime", time);
        return this;
    }

    public ITerminalResponseBuilder AddToPing(string toPing)
    {
        _attributes["to_ping"] = new XAttribute("to_ping", toPing);
        return this;
    }

    public ITerminalResponseBuilder AddToCmd(string code)
    {
        _attributes["to_cmd"] = new XAttribute("to_cmd", code);
        return this;
    }

    public ITerminalResponseBuilder AddQ(string q)
    {
        _attributes[q] = new XAttribute("q", q);
        return this;
    }

    public ITerminalResponseBuilder AddGetSettings(string code)
    {
        _attributes["get_settings"] = new XAttribute("get_settings", code);
        return this;
    }

    public ITerminalResponseBuilder AddFirstHost()
    {
        _attributes["s1"] = new XAttribute("s1", _host);
        return this;
    }

    public ITerminalResponseBuilder AddSecondHost()
    {
        _attributes["s2"] = new XAttribute("s2", _host);
        return this;
    }

    public ITerminalResponseBuilder AddFirstPort()
    {
        _attributes["p1"] = new XAttribute("p1", _port);
        return this;
    }

    public ITerminalResponseBuilder AddFirstPortE()
    {
        _attributes["p1e"] = new XAttribute("p1e", _port);
        return this;
    }

    public ITerminalResponseBuilder AddSecondPort()
    {
        _attributes["p2"] = new XAttribute("p2", _port);
        return this;
    }

    public ITerminalResponseBuilder AddSecondPortE()
    {
        _attributes["p2e"] = new XAttribute("p2e", _port);
        return this;
    }

    public XDocument Build()
    {
        var xDocument = _xDocument;
        var row = new XElement("ROW", _attributes.Values);
        xDocument.Element("DP")!.Element("R")!.Add(row);
        Reset();
        return xDocument;
    }

    private void Reset()
    {
        _xDocument = new XDocument(
            new XElement("DP",
                new XElement("M",
                    new XElement("S",
                        new XAttribute("serv1", _host),
                        new XAttribute("portf1", _port),
                        new XAttribute("porte1", _port),
                        new XAttribute("serv2", _host),
                        new XAttribute("portf2", _port),
                        new XAttribute("porte2", _port))),
                new XElement("R")));

        _attributes.Clear();
    }
}