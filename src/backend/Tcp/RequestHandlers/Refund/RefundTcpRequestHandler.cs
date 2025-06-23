using System.Globalization;
using System.Xml.Linq;
using Application.Refunds.Commands;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;
using Tcp.Abstractions;
using Tcp.Extensions;
using Tcp.Helpers.CheckBuilder;
using Tcp.Helpers.TerminalResponseBuilder;

namespace Tcp.RequestHandlers.Refund;

public class RefundTcpRequestHandler : ITcpRequestHandler<RefundTcpRequest>
{
    private readonly IMediator _mediator;
    private readonly TcpOptions _tcpOptions;

    public RefundTcpRequestHandler(IMediator mediator, IOptions<TcpOptions> tcpOptions)
    {
        _mediator = mediator;
        _tcpOptions = tcpOptions.Value;
    }

    public async Task<Result<XDocument>> HandleAsync(RefundTcpRequest request, CancellationToken cancellationToken = default)
    {
        var requestBody = request.R.Row;

        if (!decimal.TryParse(requestBody.ProductPrice, CultureInfo.InvariantCulture, out var price))
            return Result.Fail("Failed to parse product price");
        if (!decimal.TryParse(requestBody.Quantity, CultureInfo.InvariantCulture, out var quantity))
            return Result.Fail("Failed to parse product quantity");

        CreateRefundCommand command = new(requestBody.CardNumber, requestBody.ProductId, quantity, price);
        
         var result = await _mediator.Send(command, cancellationToken);

         if (result.IsFailed)
             return ProcessFailed(result);
        
        var settings = new TerminalResponseBuilderSettings(_tcpOptions.Host, _tcpOptions.Port.ToString());
        var responseBuilder = new TerminalResponseBuilder(settings);
        var checkBuilder = new CheckBuilder();

        var check = checkBuilder
            .AddSeparator()
            .AddLine("       Svoy.Club         ")
            .AddLine("    Processing center    ")
            .AddSeparator()
            .AddLine("ООО `Иванов Иван Иван`")
            .AddLine("ИНН 0000000000 ТО :")
            .AddLine(" Красноармейск")
            .AddLine(" ул. 1 Мая,  5")
            .AddSeparator()
            .AddLine($"   {requestBody.Date}   ")
            .AddLine("ТО .................28140")
            .AddLine("ЭмитТО ..............0010")
            .AddLine($"Карта № .....{requestBody.CardNumber}")
            .AddLine($"Чек № ..................{result.Value.CheckId}")
            .AddLine($"Баланс ............{result.Value.CardBalance}")
            .AddSeparator()
            .AddLine($"{result.Value.ProductName} (Возв.)========-{requestBody.Quantity}")
            .AddLine($"Цена :{requestBody.ProductPrice}")
            .AddLine($"Сумма :-{requestBody.Total}")
            .AddLine("Добро пожаловать!")
            .AddLine("***********MPay**********")
            .AddSeparator()
            .AddIndent()
            .Build();

        var response = responseBuilder
            .AddResponseCode("0")
            .AddOFlResponse("0")
            .AddRejection("0")
            .AddRejectionCode("0")
            .AddCheckId(result.Value.CheckId.ToString())
            .AddBalance(result.Value.CardBalance.ToString(CultureInfo.InvariantCulture))
            .AddCheck(check)
            .AddCardNumber(requestBody.CardNumber)
            .Build();

        return Result.Ok(response);
    }
    
    private Result<XDocument> ProcessFailed(ResultBase result)
    {
        if (!result.TryGetTerminalErrorCode(out var errorCode))
            return Result.Fail("Failed to determinate error code");
        
        var settings = new TerminalResponseBuilderSettings(_tcpOptions.Host, _tcpOptions.Port.ToString());
        var responseBuilder = new TerminalResponseBuilder(settings);
        
        var response = responseBuilder
            .AddResponseCode(errorCode!)
            .AddOFlResponse("0")
            .AddRejection("0")
            .AddRejectionCode("0")
            .AddCheckId("")
            .AddBalance("")
            .AddCheck("")
            .AddCardNumber("")
            .Build();

        return Result.Ok(response);
    }
}