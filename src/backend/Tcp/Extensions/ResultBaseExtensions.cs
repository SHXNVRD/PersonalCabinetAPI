using Domain.Shared.Errors;
using FluentResults;

namespace Tcp.Extensions;

public static class ResultBaseExtensions
{
    public static bool TryGetTerminalErrorCode(this ResultBase result, out string? errorCode)
    {
        errorCode = null;
        
        if (result.IsSuccess)
            return false;

        foreach (var error in result.Errors.OfType<ConflictError>())
        {
            errorCode = error.GetTerminalErrorCode();
            if (errorCode != null)
                return true;
        }

        return false;
    }
}