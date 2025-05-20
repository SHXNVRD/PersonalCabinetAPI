using FluentValidation;

namespace API.Controllers.User.DTOs;

public class GetUsersRequestValidator : AbstractValidator<GetUsersRequest>
{
    public GetUsersRequestValidator()
    {
        RuleFor(r => r.Page)
            .GreaterThan(0).WithMessage("Page must be greater than 0");

        RuleFor(r => r.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0");
    }
}