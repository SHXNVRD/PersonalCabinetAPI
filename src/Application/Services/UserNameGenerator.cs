using Application.Extensions;

namespace Application.Services;

public static class UserNameGenerator
{
    public static string GenerateByEmail(string email, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
    {
        if (!email.IsEmail())
            throw new ArgumentException($"Parameter {nameof(email)} must represent email address string");

        var index = email.IndexOf('@', comparisonType);
        
        return email[..index].ToLowerInvariant();
    }
}