using System.Runtime.InteropServices.JavaScript;
using Domain.Aggregates.Base;
using FluentResults;

namespace Domain.Aggregates.PurchaseAggregate;

public sealed class Check : Identity<long>
{
    public DateTime CreatedAt { get; private set; }
    
    private Check()
    { }

    private Check(DateTime createdAt) : this()
    {
        CreatedAt = createdAt;
    }

    public static Result<Check> Create()
    {
        return new Check(DateTime.UtcNow);
    }
}