using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace API.Middlewares
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly IProblemDetailsService _problemDetailsService;
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IHostEnvironment _environment;

        public GlobalExceptionHandler(
            ILogger<GlobalExceptionHandler> logger, 
            IProblemDetailsService problemDetailsService, 
            IHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _problemDetailsService = problemDetailsService;
            _environment = hostEnvironment;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "An unhandled exception has occurred while executing the request: {Message}", exception.Message);

            var detail = _environment.IsDevelopment() 
                ? exception.Message
                : "An internal server error has occurred.";
            
            return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails =
                {
                    Type = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.6.1",
                    Title = "Internal server error",
                    Detail = detail,
                    Instance = httpContext.Request.Path.Value
                }
            });
        }
    }
}