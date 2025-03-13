using Domain.Aggregates.CardAggregate;
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

        public Result AddCard(Card card)
        {
            if (card == null)
                return Result.Fail($"{nameof(card)} cannot be null");
            if (_cards.Any(c => c.Number == card.Number))
                return Result.Fail("Duplicated card");

            var activationResult = card.Activate(Id);
            if (activationResult.IsFailed)
                return Result.Fail(activationResult.Errors);
            
            _cards.Add(card);

            return Result.Ok();
        }
    }
}