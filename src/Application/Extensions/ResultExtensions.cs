using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Application.Extensions
{
    public static class ResultExtensions
    {
        public static Result<T> ToFluentResult<T>(this IdentityResult result)
        {
            if (result.Errors.Count() < 1)
                throw new NullReferenceException("Result do not have errors");

            var fluentResult = new Result<T>();

            if (result.Succeeded)
                return fluentResult;
            
            var errors = result.Errors.Select(e => new Error(e.Description));
            fluentResult.WithErrors(errors);

            return fluentResult;
        }

        public static NotFoundObjectResult ToNotFoundResult(this ResultBase result)
        {
            if (result.IsSuccess)
                throw new Exception($"Result {result} must be failed");

            var errors = result.Errors.Select(e => e.Message);        

            return new NotFoundObjectResult(errors);
        }

        public static UnauthorizedObjectResult ToUnauthorizedResult(this ResultBase result)
        {
            if (result.IsSuccess)
                throw new Exception($"Result {result} must be failed");

            var errors = result.Errors.Select(e => e.Message);        

            return new UnauthorizedObjectResult(errors);
        }
    }
}