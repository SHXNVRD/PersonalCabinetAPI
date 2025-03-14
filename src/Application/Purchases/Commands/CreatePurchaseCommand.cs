using FluentResults;
using MediatR;

namespace Application.Purchases.Commands;

public record CreatePurchaseCommand(
    string CardNumber,
    string CardPin,
    long ProductId,
    decimal ProductPrice,
    double Quantity) : IRequest<Result<CreatePurchaseResponse>>;