using API.Extensions;
using Domain.Shared.ValueObjects;
using FluentValidation;

namespace API.Controllers.Card.DTOs;

public class BlockCardRequestValidator : AbstractValidator<BlockCardRequest>
{
    public BlockCardRequestValidator()
    {
        RuleFor(r => r.CardId).MustBeGuid();
    }
}