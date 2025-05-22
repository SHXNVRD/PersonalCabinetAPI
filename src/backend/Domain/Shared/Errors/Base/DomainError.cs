using FluentResults;
using Microsoft.AspNetCore.Http;

namespace Domain.Shared.Errors.Base;

public class DomainError : Error
{
    public const string ErrorCodeMetadataKey = "ErrorCode";
    
    protected DomainError(string message, ErrorCode code)
        : base(message)
    {
        WithMetadata(ErrorCodeMetadataKey, code);
    }
}