using Application.Purchases.DTOs;
using FluentResults;
using MediatR;

namespace Application.Purchases.Commands;

public class CreatePurchaseCommand : IRequest<Result<CreatePurchaseResponse>>
{
    public required string CardNumber { get; init; }
    public required string PinCode { get; init; }
    public required int ProductId { get; init; }
    public required int Quantity { get; init; }
    public DateTime CreatedAt { get; init; }
}