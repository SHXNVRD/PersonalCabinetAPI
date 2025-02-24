using System.Globalization;
using System.Xml.Linq;
using Application.Purchases.Commands;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tcp.Abstractions;
using Tcp.DTOs.CreatePurchase;

namespace Tcp.RequestHandlers;

public class CreatePurchaseTcpRequestHandler : ITcpRequestHandler<CreatePurchaseTcpRequest>
{
    private readonly IMediator _mediator;
    private readonly TcpOptions _tcpOptions;
    private readonly ILogger<CreatePurchaseCommand> _logger;

    public CreatePurchaseTcpRequestHandler(IMediator mediator, IOptions<TcpOptions> options, ILogger<CreatePurchaseCommand> logger)
    {
        _mediator = mediator;
        _logger = logger;
        _tcpOptions = options.Value;
    }

    public async Task<Result<XDocument>> HandleAsync(CreatePurchaseTcpRequest request, CancellationToken cancellationToken = default)
    {
        var requestBody = request.R.Row;
        
        CreatePurchaseCommand command = new()
        {
            ProductId = requestBody.ProductId,
            CardNumber = requestBody.CardNumber,
            Quantity = requestBody.Quantity,
            CreatedAt = DateTime.Parse(requestBody.Date, DateTimeFormatInfo.InvariantInfo),
            PinCode = requestBody.CardPinCode
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
                           $"#13;#10;   {requestBody.Date}   " +
                           "#13;#10;ТО .................28140" +
                           "#13;#10;ЭмитТО ..............0010" +
                           $"#13;#10;Карта № .....{requestBody.CardNumber}" +
                           $"#13;#10;Чек № ..................{result.Value.CheckId}" +
                           $"#13;#10;Баланс ............{result.Value.CardBalance}" +
                           "#13;#10;*************************" +
                           $"#13;#10;{result.Value.ProductName} (Деб.)========{requestBody.Quantity}" +
                           $"#13;#10;Цена :{requestBody.ProductPrice}" +
                           $"#13;#10;Сумма :{requestBody.Total}" +
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