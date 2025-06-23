using API.Extensions;
using Domain.Aggregates.ProductAggregate;
using Domain.Shared.ValueObjects;
using FluentValidation;

namespace API.Controllers.Product.DTOs;

public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(r => r.CategoryId)
            .Must(id => Category.FromId(id).IsSuccess).WithMessage("Category with specified id not found");

        RuleFor(r => r.Title).NotEmpty().WithMessage("Product title is required");

        RuleFor(r => r.Description).NotEmpty().WithMessage("Product description is required");

        RuleFor(r => r.Quantity).MustBeValueObject(Quantity.Create);

        RuleFor(r => r.Price).GreaterThan(0).WithMessage("Product price must be greater then zero");
    }
}