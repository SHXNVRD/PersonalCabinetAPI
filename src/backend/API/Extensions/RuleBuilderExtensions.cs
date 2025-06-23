using CSharpFunctionalExtensions;
using FluentValidation;

namespace API.Extensions;
    
public static class RuleBuilderExtensions
{
    public static IRuleBuilderOptions<T, string> MustBeValueObject<T, TValueObject>(
        this IRuleBuilder<T, string> ruleBuilder, Func<string, FluentResults.Result<TValueObject>> factoryMethod) 
        where TValueObject : ValueObject
    {
        return (IRuleBuilderOptions<T, string>)ruleBuilder.Custom((value, context) =>
        {
            var result = factoryMethod(value);
            if (result.IsFailed)
                context.AddFailure(result.Errors.First().Message);
        });
    }
    
    public static IRuleBuilderOptions<T, decimal> MustBeValueObject<T, TValueObject>(
        this IRuleBuilder<T, decimal> ruleBuilder, Func<decimal, FluentResults.Result<TValueObject>> factoryMethod) 
        where TValueObject : ValueObject
    {
        return (IRuleBuilderOptions<T, decimal>)ruleBuilder.Custom((value, context) =>
        {
            var result = factoryMethod(value);
            if (result.IsFailed)
                context.AddFailure(result.Errors.First().Message);
        });
    }
}

