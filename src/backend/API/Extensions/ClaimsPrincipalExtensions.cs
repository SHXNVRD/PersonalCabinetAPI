using System.Security.Claims;
using Domain.Shared.Errors;
using FluentResults;

namespace API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Result<string> GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (userId == null)
            return Result.Fail(Errors.Unauthorized.WrongCredentials("Invalid access token"));

        return userId;
    }
}