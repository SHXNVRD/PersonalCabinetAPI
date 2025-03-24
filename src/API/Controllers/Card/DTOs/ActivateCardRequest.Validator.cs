using API.Extensions;
using Domain.Shared.ValueObjects;
using FluentValidation;

namespace API.Controllers.Card.DTOs;

public class ActivateCardRequestValidator : AbstractValidator<ActivateCardRequest>
{
    public ActivateCardRequestValidator()
    {
        RuleFor(r => r.CardPinCode).MustBeValueObject(CardPinHash.Create);

        RuleFor(r => r.CardNumber).MustBeValueObject(CardNumber.Create);
    }
}