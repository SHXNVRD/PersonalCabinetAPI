using FluentResults;
using Microsoft.AspNetCore.Identity;

namespace Application.Extensions;

public static class IdentityResultExtensions
{
    public static Result<T> ToFluentResult<T>(this IdentityResult result)
    {
        if (result.Succeeded)
            return new Result<T>();

        return ProcessFailedResult<T>(result);
    }

    private static Result<T> ProcessFailedResult<T>(IdentityResult result)
    {
        if (result.Succeeded)
            throw new InvalidCastException($"{nameof(result)} must be failed");
        if (!result.Errors.Any())
            return Result.Fail<T>("Undefined error");

        return Result.Fail<T>(result.Errors.Select(e => e.ToDomainError()));
    }
    
    public static Result ToFluentResult(this IdentityResult result)
    {
        if (result.Succeeded)
            return new Result();

        return ProcessFailedResult(result);
    }

    private static Result ProcessFailedResult(IdentityResult result)
    {
        if (result.Succeeded)
            throw new InvalidCastException($"{nameof(result)} must be failed");
        if (!result.Errors.Any())
            return Result.Fail("Undefined error");

        return Result.Fail(result.Errors.Select(e => e.ToDomainError()));
    }
}