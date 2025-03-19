using System.Xml.Linq;

namespace Tcp.Helpers.TerminalResponseBuilder;

public interface ITerminalResponseBuilder
{
    ITerminalResponseBuilder AddResponseCode(string code);
    ITerminalResponseBuilder AddOFlResponse(string code);
    ITerminalResponseBuilder AddRejection(string rejection);
    ITerminalResponseBuilder AddRejectionCode(string code);
    ITerminalResponseBuilder AddBalance(string balance);
    ITerminalResponseBuilder AddCheckId(string id);
    ITerminalResponseBuilder AddCheck(string check);
    ITerminalResponseBuilder AddCardNumber(string number);
    ITerminalResponseBuilder AddTestMessage(string message);
    ITerminalResponseBuilder AddPingTime(string time);
    ITerminalResponseBuilder AddToPing(string toPing);
    ITerminalResponseBuilder AddToCmd(string code);
    ITerminalResponseBuilder AddQ(string q);
    ITerminalResponseBuilder AddGetSettings(string code);
    ITerminalResponseBuilder AddFirstHost();
    ITerminalResponseBuilder AddSecondHost();
    ITerminalResponseBuilder AddFirstPort();
    ITerminalResponseBuilder AddFirstPortE();
    ITerminalResponseBuilder AddSecondPort();
    ITerminalResponseBuilder AddSecondPortE();
    XDocument Build();
}