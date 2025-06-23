using API.Base;
using Microsoft.AspNetCore.Mvc;

namespace API.Helpers.ProblemDetailsBuilder;

public class ProblemDetailsBuilder : IProblemDetailsBuilder
{
    private readonly int _statusCode;
    private ProblemDetails _problemDetails = new();

    public ProblemDetailsBuilder(int statusCode)
    {
        _statusCode = statusCode;   
    }

    public IProblemDetailsBuilder AddTitle(string? title = null)
    {
        _problemDetails.Title = title ?? _statusCode switch
        {
            StatusCodes.Status400BadRequest => ProblemDetailsTitles.BadRequestTitle,
            StatusCodes.Status401Unauthorized => ProblemDetailsTitles.UnauthorizedTitle,
            StatusCodes.Status403Forbidden => ProblemDetailsTitles.ForbiddenTitle,
            StatusCodes.Status404NotFound => ProblemDetailsTitles.NotFoundTitle,
            StatusCodes.Status409Conflict => ProblemDetailsTitles.ConflictTitle,
            StatusCodes.Status500InternalServerError => ProblemDetailsTitles.InternalServerErrorTitle,
            _ => throw new ArgumentOutOfRangeException(nameof(_statusCode))
        };

        return this;
    }

    public IProblemDetailsBuilder AddDetail(string? detail = null)
    {
        _problemDetails.Detail = detail ?? _statusCode switch
        {
            StatusCodes.Status400BadRequest => ProblemDetailsDetails.BadRequestDetail,
            StatusCodes.Status401Unauthorized => ProblemDetailsDetails.UnauthorizedDetail,
            StatusCodes.Status403Forbidden => ProblemDetailsDetails.ForbiddenDetail,
            StatusCodes.Status404NotFound => ProblemDetailsDetails.NotFoundDetail,
            StatusCodes.Status409Conflict => ProblemDetailsDetails.ConflictDetail,
            StatusCodes.Status500InternalServerError => ProblemDetailsDetails.InternalServerErrorDetail,
            _ => throw new ArgumentOutOfRangeException(nameof(_statusCode))
        };
        
        return this;
    }

    public IProblemDetailsBuilder AddType(string? type = null)
    {
        _problemDetails.Type = type ?? _statusCode switch
        {
            StatusCodes.Status400BadRequest => ProblemDetailsTypes.BadRequestType,
            StatusCodes.Status401Unauthorized => ProblemDetailsTypes.UnauthorizedType,
            StatusCodes.Status403Forbidden => ProblemDetailsTypes.NotFoundType,
            StatusCodes.Status404NotFound => ProblemDetailsTypes.NotFoundType,
            StatusCodes.Status409Conflict => ProblemDetailsTypes.ConflictType,
            StatusCodes.Status500InternalServerError => ProblemDetailsTypes.InternalServerErrorType,
            _ => throw new ArgumentOutOfRangeException(nameof(_statusCode))
        };
        
        return this;
    }

    public IProblemDetailsBuilder AddInstance(string instance)
    {
        _problemDetails.Instance = instance;
        return this;
    }

    public IProblemDetailsBuilder AddExtension(string key, object value)
    {
        _problemDetails.Extensions.Add(key, value);
        return this;
    }

    public ProblemDetails Build()
    {
        var problemDetails = _problemDetails;
        _problemDetails = new ProblemDetails();
        return problemDetails;
    }
}   