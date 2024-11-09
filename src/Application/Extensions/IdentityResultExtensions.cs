using FluentResults;
using Microsoft.AspNetCore.Identity;

namespace Application.Extensions
{
    public static class IdentityResultExtensions
    {
        public static Result<T> ToFluentResult<T>(this IdentityResult result)
        {
            if (!result.Errors.Any())
                throw new InvalidCastException("Result do not have errors");

            var fluentResult = new Result<T>();

            if (result.Succeeded)
                return fluentResult;
            
            var errors = result.Errors.Select(e => new Error(e.Description));
            fluentResult.WithErrors(errors);

            return fluentResult;
        }
        
        public static Result ToFluentResult(this IdentityResult result)
        {
            if (!result.Errors.Any())
                throw new InvalidCastException("Result do not have errors");

            var fluentResult = new Result();

            if (result.Succeeded)
                return fluentResult;
            
            var errors = result.Errors.Select(e => new Error(e.Description));
            fluentResult.WithErrors(errors);

            return fluentResult;
        }
    }
}