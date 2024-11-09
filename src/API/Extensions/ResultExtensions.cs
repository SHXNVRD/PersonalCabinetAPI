using System.Text;
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
        
        public static BadRequestObjectResult ToBadRequestResult(this ResultBase result)
        {
            if (result.IsSuccess)
                throw new InvalidCastException($"Result must be failed");
    
            var problem = new ProblemDetails
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.1",
                Title = "Bad Request",
                Detail = "The request is invalid.",
                Instance = _httpContextAccessor.HttpContext!.Request.Path.Value
            };
            var errors = result.Errors.Select(e => e.Message);
            problem.Extensions.Add("errors", errors);

            return new BadRequestObjectResult(problem);
        }
        
        public static ObjectResult ToForbiddenResult(this ResultBase result)
        {
            if (result.IsSuccess)
                throw new InvalidCastException($"Result must be failed");
    
            var problem = new ProblemDetails
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.4",
                Title = "Forbidden",
                Detail = "Access to this resource is denied.",
                Instance = _httpContextAccessor.HttpContext!.Request.Path.Value
            };
            var errors = result.Errors.Select(e => e.Message);
            problem.Extensions.Add("errors", errors);
            
            return new ObjectResult(problem) { StatusCode = StatusCodes.Status403Forbidden };
        }
        
        public static NotFoundObjectResult ToNotFoundResult(this ResultBase result)
        {
            if (result.IsSuccess)
                throw new InvalidCastException($"Result must be failed");

            var problem = new ProblemDetails
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.5",
                Title = "Not Found",
                Detail = "The requested resource was not found.",
                Instance = _httpContextAccessor.HttpContext!.Request.Path.Value
            };
            var errors = result.Errors.Select(e => e.Message);
            problem.Extensions.Add("errors", errors);

            return new NotFoundObjectResult(problem);
        }

        public static UnauthorizedObjectResult ToUnauthorizedResult(this ResultBase result)
        {
            if (result.IsSuccess)
                throw new InvalidCastException($"Result must be failed");
            
            var problem = new ProblemDetails
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.2",
                Title = "Unauthorized",
                Detail = "Failed to log in. Authorization is required.",
                Instance = _httpContextAccessor.HttpContext!.Request.Path.Value
            };
            var errors = result.Errors.Select(e => e.Message);
            problem.Extensions.Add("errors", errors);

            return new UnauthorizedObjectResult(problem);
        }

        public static ConflictObjectResult ToConflictResult(this ResultBase result)
        {
            if (result.IsSuccess)
                throw new InvalidCastException($"Result must be failed");
            
            var problem = new ProblemDetails
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.10",
                Title = "Conflict",
                Detail = "Failed to create new resource.",
                Instance = _httpContextAccessor.HttpContext!.Request.Path.Value
            };
            var errors = result.Errors.Select(e => e.Message);
            problem.Extensions.Add("errors", errors);

            return new ConflictObjectResult(problem);
        }
    }
}