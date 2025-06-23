using FluentResults;
using MediatR;

namespace Application.Refunds.Commands;

public record CreateRefundCommand(
    string CardNumber,
    long ProductId,
    decimal Quantity,
    decimal ProductPrice) : IRequest<Result<CreateRefundResponse>>;