namespace Application.Purchases.Commands;

public record CreatePurchaseResponse(
    decimal CardBalance,
    long CheckId,
    string ProductName);