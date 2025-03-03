using Application.Refunds.DTOs;
using FluentResults;
using MediatR;

namespace Application.Refunds.Commands;

public class CreateRefundCommand : IRequest<Result<CreateRefundResponse>>
{
    public string CardNumber { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal ProductPrice { get; set; }
    public decimal Total { get; set; }
    public DateTime RefundedAt { get; set; }
}