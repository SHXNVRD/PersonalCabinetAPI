namespace Application.Refunds.DTOs;

public class CreateRefundResponse
{
    public decimal CardBalance { get; set; }
    public long CheckId { get; set; }
    public string ProductName { get; set; }
}