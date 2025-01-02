using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using API.Abstractions;
using API.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Macs;
using Serilog;

namespace API.Services;

public class TcpServer : ITcpServer
{
    private readonly TcpListener _tcpListener;

    public TcpServer()
    {
        _tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8002);
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _tcpListener.Start();

            while (!cancellationToken.IsCancellationRequested)
            {
                var tcpClient = await _tcpListener.AcceptTcpClientAsync(cancellationToken);
                Task.Run(async () => await HandleClientAsync(tcpClient), cancellationToken);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    private Task HandleClientAsync(TcpClient tcpClient)
    {
        try
        {
            var buffer = tcpClient
                .GetStream()
                .ReadUntilTimeout(TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(20));
            
            var data = Encoding.UTF8.GetString(buffer);
            Console.WriteLine(data);
            var xml = XDocument.Parse(data);
            var rowElement = xml
                .Element("DP")
                ?.Element("R")
                ?.Element("ROW");

            if (rowElement == null)
                return Task.CompletedTask;

            switch (rowElement.Attribute("cmdtype")!.Value)
            {
                case "2000": SendPing(tcpClient);
                    break;
            }
        }
        catch (IOException ioe) when (ioe.InnerException is SocketException soe)
        {
            Log.Logger.Error(soe.Message);
        }
        finally
        {
            tcpClient.Close();
        }
        
        return Task.CompletedTask;
    }

    private void SendPing(TcpClient tcpClient)
    {
        var localIp = ((IPEndPoint)tcpClient.Client.LocalEndPoint!).Address.ToString();
        var localPort = ((IPEndPoint)tcpClient.Client.LocalEndPoint!).Port.ToString();

        var response = new XDocument(
            new XElement("DP",
                new XElement("M",
                    new XElement("S",
                        new XAttribute("serv1", localIp),
                        new XAttribute("portf1", localPort),
                        new XAttribute("porte1", localPort),
                        new XAttribute("serv2", localIp),
                        new XAttribute("portf2", localPort),
                        new XAttribute("porte2", localPort))),
                new XElement("R",
                    new XElement("ROW",
                        new XAttribute("kod_otvet_xml", "0"),
                        new XAttribute("test_mes", "Тестовый чек#13;#10; Строка 2 #13;#10;"),
                        new XAttribute("pingtime", "60"), // заменить на реальное время
                        new XAttribute("get_settings", "0"),
                        new XAttribute("to_ping", "2"),
                        new XAttribute("to_cmd", "30"),
                        new XAttribute("q", "0"),
                        new XAttribute("s1", localIp),
                        new XAttribute("s2", localIp),
                        new XAttribute("p1", localPort),
                        new XAttribute("p2", localPort),
                        new XAttribute("p1e", localPort),
                        new XAttribute("p2e", localPort)))));
        
        SendResponse(tcpClient, response);
    }

    private void SendResponse(TcpClient tcpClient, XDocument response)
    {
        var buffer = response.ToByteArray(Encoding.GetEncoding(1251));
        Console.WriteLine(Encoding.GetEncoding(1251).GetString(buffer));
        var stream = tcpClient.GetStream();
        stream.Write(buffer, 0, buffer.Length);
        stream.Flush();
    }
}