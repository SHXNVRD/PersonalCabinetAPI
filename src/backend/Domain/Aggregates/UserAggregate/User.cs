using System.Security.Principal;
using Domain.Aggregates.Base;
using Domain.Aggregates.CardAggregate;
using Domain.Shared.Errors;
using Domain.Shared.ValueObjects;
using FluentResults;
using Microsoft.AspNetCore.Identity;

namespace Domain.Aggregates.UserAggregate;

public class User : IdentityUser<Guid>
{
    public DateTime RegisteredAt { get; private set; } = DateTime.UtcNow;
    public DateOnly? DayOfBirth { get; private set; }
    private List<Card> _cards = [];
    public IReadOnlyList<Card> Cards => _cards.AsReadOnly();
    public Name Name { get; set; }
    
    private User()
    { }

    private User(
        string email,
        string phoneNumber,
        string userName,
        Name name) : this()
    {
        Email = email;
        PhoneNumber = phoneNumber;
        UserName = userName;
        Name = name;
    }

    public static Result<User> Create(string email, string phoneNumber, string userName, Name name)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Result.Fail(new InvalidData($"{nameof(email)} cannot be empty"));
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return Result.Fail(new InvalidData($"{nameof(phoneNumber)} cannot be empty"));
        if (string.IsNullOrWhiteSpace(userName))
            return Result.Fail(new InvalidData($"{nameof(userName)} cannot be empty"));
        if (name is null)
            return Result.Fail(new InvalidData($"{nameof(name)} cannot be empty"));

        return new User(email.Trim(), phoneNumber.Trim(), userName.Trim(), name);
    }
        
    public Result BlockCard(Guid cardId)
    {
        if (cardId == Guid.Empty)
            return Result.Fail(new InvalidData($"{nameof(cardId)} cannot be empty"));

        var card = _cards.SingleOrDefault(c => c.Id == cardId);
        if (card == null)
            return Result.Fail(new NotFound($"Card with id {cardId} was not found"));

        card.Block();
            
        return Result.Ok();
    }

    public Result FreezeCard(Guid cardId)
    {
        if (cardId == Guid.Empty)
            return Result.Fail(new InvalidData($"{nameof(cardId)} cannot be empty"));

        var card = _cards.SingleOrDefault(c => c.Id == cardId);
        if (card == null)
            return Result.Fail(new NotFound($"Card with id {cardId} was not found"));

        card.Freeze();
            
        return Result.Ok();
    }
    
    public Result UnFreezeCard(Guid cardId)
    {
        if (cardId == Guid.Empty)
            return Result.Fail(new InvalidData($"{nameof(cardId)} cannot be empty"));

        var card = _cards.SingleOrDefault(c => c.Id == cardId);
        if (card == null)
            return Result.Fail(new NotFound($"Card with id {cardId} was not found"));

        card.UnFreeze();
            
        return Result.Ok();
    }
}