using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentResults;
using Microsoft.AspNetCore.Identity;

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
            
            foreach (var error in result.Errors)
            {
                fluentResult.WithError(new Error(error.Description));
            }

            return fluentResult;
        }
    }
}