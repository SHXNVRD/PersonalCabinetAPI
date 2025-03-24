using API.Extensions;
using Application.Cards.Queries;
using FluentValidation;

namespace API.Controllers.Card.DTOs;

public class GetOperationsByIdRequestValidator : AbstractValidator<GetOperationsByIdRequest>
{
    public GetOperationsByIdRequestValidator()
    {
        RuleFor(r => r.CardId)
            .NotEqual(Guid.Empty).WithMessage("Card id cannot is required");

        RuleFor(r => r.Page)
            .GreaterThan(0).WithMessage("Page must be greater than 0");

        RuleFor(r => r.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0");
    }
}