namespace Application.Purchases.DTOs;

public class CreatePurchaseResponse
{
    public long CheckId { get; set; }
    public decimal CardBalance { get; set; }
    public string ProductName { get; set; }
}