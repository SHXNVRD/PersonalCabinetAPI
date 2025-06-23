using System.Globalization;
using System.Xml.Linq;
using Application.Purchases.Commands;
using Domain.Shared.Errors;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;
using Tcp.Abstractions;
using Tcp.Extensions;
using Tcp.Helpers.CheckBuilder;
using Tcp.Helpers.TerminalResponseBuilder;

namespace Tcp.RequestHandlers.CreatePurchase;

public class CreatePurchaseTcpRequestHandler : ITcpRequestHandler<CreatePurchaseTcpRequest>
{
    private readonly IMediator _mediator;
    private readonly TcpOptions _tcpOptions;

    public CreatePurchaseTcpRequestHandler(IMediator mediator, IOptions<TcpOptions> options)
    {
        _mediator = mediator;
        _tcpOptions = options.Value;
    }

    public async Task<Result<XDocument>> HandleAsync(CreatePurchaseTcpRequest request, CancellationToken cancellationToken = default)
    {
        var requestBody = request.R.Row;

        if (!decimal.TryParse(requestBody.Quantity, CultureInfo.InvariantCulture, out var quantity))
            return Result.Fail(Errors.InvalidData.ValidationFailed($"Invalid quantity of product: {requestBody.Quantity}"));
        if (!decimal.TryParse(requestBody.ProductPrice, CultureInfo.InvariantCulture, out var price))
            return Result.Fail(new InvalidDataError($"Invalid price of product: {requestBody.ProductPrice}"));
        
        CreatePurchaseCommand command = new(
            requestBody.CardNumber, 
            requestBody.CardPin, 
            requestBody.ProductId,
            price,
            quantity);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailed)
            return ProcessFailed(result);

        var settings = new TerminalResponseBuilderSettings(_tcpOptions.Host, _tcpOptions.Port.ToString());
        var responseBuilder = new TerminalResponseBuilder(settings);
        var checkBuilder = new CheckBuilder();
        
        var checkContent = checkBuilder
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
            .AddLine($"{result.Value.ProductName} (Деб.)========{requestBody.Quantity}")
            .AddLine($"Цена :{requestBody.ProductPrice}")
            .AddLine($"Сумма :{requestBody.Total}")
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
            .AddCheck(checkContent)
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