using System.Net;
using Domain;

namespace API.Helpers;

public static class ApplicationErrorMapper
{
    private static readonly Dictionary<ErrorType, int> ErrorTypeToStatusCode = new()
    {
        [ErrorType.ValidationFailed] = StatusCodes.Status400BadRequest,
        [ErrorType.Unauthorized] = StatusCodes.Status401Unauthorized,
        [ErrorType.Forbidden] = StatusCodes.Status403Forbidden,
        [ErrorType.NotFound] = StatusCodes.Status404NotFound,
        [ErrorType.Conflict] = StatusCodes.Status409Conflict
    };

    public static int MapToStatusCode(ErrorType errorType)
        => ErrorTypeToStatusCode.TryGetValue(errorType, out var code) 
            ? code 
            : StatusCodes.Status500InternalServerError;
}