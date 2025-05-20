namespace Application.Refunds.Commands;

public record CreateRefundResponse(
    decimal CardBalance,
    long CheckId,
    string ProductName);