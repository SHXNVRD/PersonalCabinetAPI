using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Application.Behaviors
{
    public sealed class RequestLoggningPipelineBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : class
        where TResponse : ResultBase
    {
        private readonly ILogger<RequestLoggningPipelineBehavior<TRequest, TResponse>> _logger;

        public RequestLoggningPipelineBehavior(ILogger<RequestLoggningPipelineBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;

            _logger.LogInformation(
                "Starting requrst {RequestName}, {DateTimeUtc}",
                requestName,
                DateTime.UtcNow);
            
            var result = await next();

            if (result.IsSuccess)
            {
                _logger.LogInformation("Completed request {RequestName}, {DateTimeUtc}",
                requestName,
                DateTime.UtcNow);
            }
            else
            {
                _logger.LogInformation(
                    "Request failure {RequestName}, {@Error}, {DateTimeUtc}",
                    requestName,
                    result.Errors,
                    DateTime.UtcNow);
            }

            return result;
        }
    }
}