using FluentValidation;

namespace API.Controllers.User.DTOs;

public class GetUsersRequestValidator : AbstractValidator<GetUsersRequest>
{
    public GetUsersRequestValidator()
    {
        RuleFor(r => r)
            .Must(r => (r.Page is null && r.PageSize is null) ||
                       (r.Page is not null && r.PageSize is not null))
            .WithMessage("Page and PageSize must both be provided or both be omitted");

        RuleFor(r => r.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0")
            .When(r => r.Page is not null);

        RuleFor(r => r.PageSize)
            .GreaterThan(0)
            .WithMessage("PageSize must be greater than 0")
            .When(r => r.PageSize is not null);
    }
}