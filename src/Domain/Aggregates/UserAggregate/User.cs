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
        
    private User()
    { }

    private User(
        string email,
        string phoneNumber,
        string userName) : this()
    {
        Email = email;
        PhoneNumber = phoneNumber;
        UserName = userName;
    }

    public static Result<User> Create(string email, string phoneNumber, string userName)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Result.Fail(new InvalidData($"{nameof(email)} cannot be empty"));
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return Result.Fail(new InvalidData($"{nameof(phoneNumber)} cannot be empty"));
        if (string.IsNullOrWhiteSpace(userName))
            return Result.Fail(new InvalidData($"{nameof(userName)} cannot be empty"));

        return new User(email.Trim(), phoneNumber.Trim(), userName.Trim());
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
}