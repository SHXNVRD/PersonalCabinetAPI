using System.Globalization;
using System.Xml.Linq;
using Application.Purchases.Commands;
using Application.Tcp;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;

namespace API.Services.Tcp.CommandHandlers;

public class CreatePurchaseTcpCommandHandler : ITcpCommandHandler
{
    private readonly IMediator _mediator;
    private readonly TcpOptions _tcpOptions;
    private readonly ILogger<CreatePurchaseCommand> _logger;

    public CreatePurchaseTcpCommandHandler(IMediator mediator, IOptions<TcpOptions> options, ILogger<CreatePurchaseCommand> logger)
    {
        _mediator = mediator;
        _logger = logger;
        _tcpOptions = options.Value;
    }

    public string RequestCode => TcpRequests.CreatePurchase;
    public async Task<Result<XDocument>> HandleAsync(XDocument request, CancellationToken cancellationToken = default)
    {
        var element = request
            .Descendants("ROW")
            .First();

        var cardNumber = element.Attribute("cardno")!.Value;
        var productQuantity = (int)double.Parse(element.Attribute("kol")!.Value, CultureInfo.InvariantCulture);
        
        CreatePurchaseCommand command = new()
        {
            ProductId = int.Parse(element.Attribute("usluga")!.Value),
            CardNumber = cardNumber,
            Quantity = productQuantity,
            CreatedAt = DateTime.Parse(element.Attribute("dt")!.Value, DateTimeFormatInfo.InvariantInfo),
            PinCode = element.Attribute("pin")!.Value
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailed)
            return Result.Fail(result.Errors);

        var checkContent = "*************************" +
                           "#13;#10;       Svoy.Club         " +
                           "#13;#10;    Processing center    " +
                           "#13;#10;*************************" +
                           "#13;#10;ООО `Иванов Иван Иван`" +
                           "#13;#10;ИНН 0000000000 ТО :" +
                           "#13;#10; Красноармейск" +
                           "#13;#10; ул. 1 Мая,  5" +
                           "#13;#10;*************************" +
                           $"#13;#10;   {element.Attribute("dt")!.Value}   " +
                           "#13;#10;ТО .................28140" +
                           "#13;#10;ЭмитТО ..............0010" +
                           $"#13;#10;Карта № .....{cardNumber}" +
                           $"#13;#10;Чек № ..................{result.Value.CheckId}" +
                           $"#13;#10;Баланс ............{result.Value.CardBalance}" +
                           "#13;#10;*************************" +
                           $"#13;#10;{result.Value.ProductName} (Деб.)========{productQuantity}" +
                           $"#13;#10;Цена :{element.Attribute("cena")!.Value}" +
                           $"#13;#10;Сумма :{element.Attribute("summa")!.Value}" +
                           "#13;#10;Добро пожаловать!" +
                           "#13;#10;***********MPay**********" +
                           "#13;#10;*************************" +
                           "#13;#10;#13;#10;#13;#10;";

        var response = new XDocument(
            new XElement("DP",
                new XElement("M",
                    new XElement("S",
                        new XAttribute("serv1", _tcpOptions.Host),
                        new XAttribute("portf1", _tcpOptions.Port),
                        new XAttribute("porte1", _tcpOptions.Port),
                        new XAttribute("serv2", _tcpOptions.Host),
                        new XAttribute("portf2", _tcpOptions.Port),
                        new XAttribute("porte2", _tcpOptions.Port))),
                new XElement("R",
                    new XElement("ROW",
                        new XAttribute("kod_otvet_xml", "0"),
                        new XAttribute("o_fl_otvet", "0"),
                        new XAttribute("otkaz", "0"),
                        new XAttribute("kod_otkaz", "0"),
                        new XAttribute("kod_check", result.Value.CheckId),
                        new XAttribute("balance", result.Value.CardBalance),
                        new XAttribute("checksrc", checkContent)))));

        return Result.Ok(response);
    }
}