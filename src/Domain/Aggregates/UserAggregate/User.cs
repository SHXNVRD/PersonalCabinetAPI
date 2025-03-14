using Domain.Aggregates.Base;
using Domain.Aggregates.CardAggregate;
using Domain.Shared.Errors;
using Domain.Shared.ValueObjects;
using FluentResults;
using Microsoft.AspNetCore.Identity;

namespace Domain.Aggregates.UserAggregate
{
    public class User : IdentityUser<Guid>
    {
        public DateTime RegisteredAt { get; private set; }
        public DateOnly? DayOfBirth { get; private set; }
        private List<Card> _cards = [];
        public IReadOnlyList<Card> Cards => _cards.AsReadOnly();
        
        public Result BlockCard(CardNumber number)
        {
            if (number is null)
                return Result.Fail(new InvalidData($"{nameof(number)} cannot be empty"));

            var card = _cards.SingleOrDefault(c => c.Number == number);
            if (card == null)
                return Result.Fail(new NotFound($"Card with number {number.Value} was not found"));

            card.Block();
            
            return Result.Ok();
        }
    }
}