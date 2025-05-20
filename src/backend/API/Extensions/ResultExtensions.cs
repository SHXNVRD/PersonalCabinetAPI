using System.Diagnostics;
using API.Helpers.ProblemDetailsBuilder;
using Domain.Shared.Errors;
using Domain.Shared.Errors.Base;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using HttpContent = Microsoft.AspNetCore.Http.HttpContext;

namespace API.Extensions;

public static class ResultExtensions
{
    public static ObjectResult ToObjectResult(this ResultBase result, HttpContent context)
    {
        if (result.IsSuccess)
            throw new InvalidCastException($"Result must be failed");

        var error = result.Errors.OfType<DomainError>().FirstOrDefault();
        var statusCode = error == null
            ? StatusCodes.Status500InternalServerError
            : MapToStatusCode(error.GetType());

        if (statusCode == StatusCodes.Status200OK)
            return new OkObjectResult(string.Empty);
            
        var errors = result.Errors
            .Select(e => e.Message)
            .ToArray();

        var builder = new ProblemDetailsBuilder(statusCode);
        var problemDetails = builder
            .AddTitle()
            .AddType()
            .AddExtension("traceId", Activity.Current?.Id ?? context.TraceIdentifier)
            .AddExtension("errors", new Dictionary<string, string[]> {{"reasons", errors}})
            .Build();

        var objectResult = new ObjectResult(problemDetails)
        {
            StatusCode = statusCode
        };

        return objectResult;
    }
    
    private static readonly Dictionary<Type, int> ErrorTypes = new()
    {
        [typeof(InvalidData)] = StatusCodes.Status400BadRequest,
        [typeof(Unauthorized)] = StatusCodes.Status401Unauthorized,
        [typeof(Forbidden)] = StatusCodes.Status403Forbidden,
        [typeof(NotFound)] = StatusCodes.Status200OK,
        [typeof(Conflict)] = StatusCodes.Status409Conflict
    };

    private static int MapToStatusCode(Type errorType)
        => ErrorTypes.TryGetValue(errorType, out var code) 
            ? code 
            : StatusCodes.Status500InternalServerError;
}