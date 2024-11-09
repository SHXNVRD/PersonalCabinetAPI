using FluentValidation;

namespace Application.Users.Commands.CreateEmailConfirmationLink
{
    public class CreateEmailConfirmationLinkCommandValidator : AbstractValidator<CreateEmailConfirmationLinkCommand>
    {
        public CreateEmailConfirmationLinkCommandValidator()
        {
            RuleFor(c => c.Email)
                .NotEmpty().WithMessage("Email address is required")
                .EmailAddress().WithMessage("Invalid email address");
        }
    }
}