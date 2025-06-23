using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Behaviors;

public sealed class RequestLoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : ResultBase
{
    private readonly ILogger<RequestLoggingBehavior<TRequest, TResponse>> _logger;
    
    public RequestLoggingBehavior(ILogger<RequestLoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation("Starting request {RequestName}", requestName);
            
        var result = await next();

        if (result.IsSuccess)
            _logger.LogInformation("Completed request {RequestName}", requestName);
        else
        {
            _logger.LogInformation(
                "Request failure {RequestName}, {@Errors}",
                requestName,
                result.Errors);
        }

        return result;
    }
}