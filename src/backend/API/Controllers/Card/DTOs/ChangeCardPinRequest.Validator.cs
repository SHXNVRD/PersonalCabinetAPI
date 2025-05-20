using System.Data;
using API.Extensions;
using Domain.Shared.ValueObjects;
using FluentValidation;

namespace API.Controllers.Card.DTOs;

public class ChangeCardPinRequestValidator : AbstractValidator<ChangeCardPinRequest>
{
    public ChangeCardPinRequestValidator()
    {
        RuleFor(r => r.Pin).MustBeValueObject(CardPinHash.Create);
    }
}