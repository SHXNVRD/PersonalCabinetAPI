using System.Diagnostics;
using API.Helpers.ProblemDetailsBuilder;
using Microsoft.AspNetCore.Diagnostics;

namespace API.Middlewares;

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
        _logger.LogCritical(
            "[EXCEPTION] type: {type}, message: {description}, exception: {@exception}, inner exception: {@innerException}",
            exception.GetType().Name, exception.Message, exception, exception.InnerException);

        var errors = _environment.IsDevelopment() 
            ? new[] {exception.Message}
            : new[] {"An internal server error has occurred."};
        
        var builder = new ProblemDetailsBuilder(StatusCodes.Status500InternalServerError);
        var problemDetails = builder
            .AddTitle()
            .AddStatus()
            .AddType()
            .AddExtension("traceId", Activity.Current?.Id ?? httpContext.TraceIdentifier)
            .AddExtension("errors", errors)
            .Build();
        
        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = problemDetails
        });
    }
}