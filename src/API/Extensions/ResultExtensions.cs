using System.Text;
using API.Helpers;
using API.Helpers.ProblemDetailsBuilder;
using Application.Errors.Base;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using InvalidCastException = System.InvalidCastException;

namespace API.Extensions
{
    public static class ResultExtensions
    {
        private static IHttpContextAccessor _httpContextAccessor;

        public static void Configure(IHttpContextAccessor httpContextAccessor) =>
            _httpContextAccessor = httpContextAccessor;

        public static ObjectResult ToObjectResult(this ResultBase result)
        {
            if (result.IsSuccess)
                throw new InvalidCastException($"Result must be failed");

            var appError = result.Errors.OfType<ApplicationError>().FirstOrDefault();
            var statusCode = appError?.ErrorType == null
                ? StatusCodes.Status500InternalServerError
                : ApplicationErrorMapper.MapToStatusCode(appError.ErrorType);
            
            var errors = result.Errors.Select(e => e.Message);

            var builder = new ProblemDetailsBuilder(statusCode);
            var problemDetails = builder
                .AddTitle()
                .AddDetail()
                .AddType()
                .AddInstance(_httpContextAccessor.HttpContext.Request.Path.Value)
                .AddExtension("errors", errors)
                .Build();

            var objectResult = new ObjectResult(problemDetails)
            {
                StatusCode = statusCode
            };

            return objectResult;
        }
    }
}