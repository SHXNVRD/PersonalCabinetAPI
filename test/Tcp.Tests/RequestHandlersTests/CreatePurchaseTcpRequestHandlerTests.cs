using System.Globalization;
using Application.Purchases.Commands;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using Tcp.RequestHandlers.CreatePurchase;

namespace Tcp.Tests.RequestHandlersTests;

public class CreatePurchaseTcpRequestHandlerTests
{
    private readonly string _quantity = "1";
    private readonly string _cardNumber = "123456789000";
    private readonly string _cardPin = "1234";
    private readonly string _date = "05.03.2025 17:50:00";
    private readonly int _productId = 1;
    private readonly string _total = "1";
    private readonly string _productPrice = "1";
    
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly Mock<IOptions<TcpOptions>> _tcpOptionsMock = new();
    
    private readonly TcpOptions _tcpOptions = new()
    {
        Host = "0.0.0.0",
        Port = 8081,
        ReadTimeout = 10,
        StartTimeout = 10
    };

    public CreatePurchaseTcpRequestHandlerTests()
    {
        _tcpOptionsMock
            .SetupGet(o => o.Value)
            .Returns(_tcpOptions);
    }

    [Fact]
    public async Task HandleAsync_MediatorReturnFail_ReturnsFail()
    {
        CreatePurchaseTcpRequest request = new()
        {
            M = default,
            R = new()
            {
                Row = new()
                {
                    Quantity = _quantity,
                    CardNumber = _cardNumber,
                    CardPin = _cardPin,
                    ProductId = _productId,
                    ProductPrice = _productPrice
                }
            }
        };
        
        var requestBody = request.R.Row;
        
        _mediatorMock.Setup(x => x.Send(
                It.Is<CreatePurchaseCommand>(c =>
                         c.Quantity == decimal.Parse(requestBody.Quantity, CultureInfo.InvariantCulture) &&
                         c.CardNumber == requestBody.CardNumber &&
                         c.CardPin == requestBody.CardPin &&
                         c.ProductId == requestBody.ProductId &&
                         c.ProductPrice == decimal.Parse(requestBody.ProductPrice, CultureInfo.InvariantCulture)),
                default))
            .ReturnsAsync(Result.Fail(It.IsAny<string>()));
        
        var handler = new CreatePurchaseTcpRequestHandler(_mediatorMock.Object, _tcpOptionsMock.Object);

        var result = await handler.HandleAsync(request, default);
        
        Assert.True(result.IsFailed);
        
        _mediatorMock.Verify(x => x.Send(
                It.Is<CreatePurchaseCommand>(c =>
                    c.Quantity == decimal.Parse(requestBody.Quantity, CultureInfo.InvariantCulture) &&
                    c.CardNumber == requestBody.CardNumber &&
                    c.CardPin == requestBody.CardPin &&
                    c.ProductId == requestBody.ProductId &&
                    c.ProductPrice == decimal.Parse(requestBody.ProductPrice, CultureInfo.InvariantCulture)),
                default),
            Times.Once);
    }
    
    [Fact]
    public async Task HandleAsync_MediatorReturnSuccess_ReturnsSuccess()
    {
        CreatePurchaseTcpRequest request = new()
        {
            M = default,
            R = new()
            {
                Row = new()
                {
                    Quantity = _quantity,
                    CardNumber = _cardNumber,
                    CardPin = _cardPin,
                    Date = _date,
                    ProductId = _productId,
                    ProductPrice = _productPrice,
                    Total = _total
                }
            }
        };

        CreatePurchaseResponse response = new(1, 1, "productName");
        
        var requestBody = request.R.Row;
        
        _mediatorMock.Setup(x => x.Send(
                It.Is<CreatePurchaseCommand>(c =>
                    c.Quantity == decimal.Parse(requestBody.Quantity, CultureInfo.InvariantCulture) &&
                    c.CardNumber == requestBody.CardNumber &&
                    c.CardPin == requestBody.CardPin &&
                    c.ProductId == requestBody.ProductId &&
                    c.ProductPrice == decimal.Parse(requestBody.ProductPrice, CultureInfo.InvariantCulture)),
                default))
            .ReturnsAsync(Result.Ok(response));
        
        var handler = new CreatePurchaseTcpRequestHandler(_mediatorMock.Object, _tcpOptionsMock.Object);

        var result = await handler.HandleAsync(request);   
        
        Assert.True(result.IsSuccess);
        
        _mediatorMock.Verify(x => x.Send(
                It.Is<CreatePurchaseCommand>(c =>
                    c.Quantity == decimal.Parse(requestBody.Quantity, CultureInfo.InvariantCulture) &&
                    c.CardNumber == requestBody.CardNumber &&
                    c.CardPin == requestBody.CardPin &&
                    c.ProductId == requestBody.ProductId &&
                    c.ProductPrice == decimal.Parse(requestBody.ProductPrice, CultureInfo.InvariantCulture)),
                default),
            Times.Once);
    }
}