using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services.Token.Providers;

public class PasswordResetTokenProviderOptions : DataProtectionTokenProviderOptions
{
    public PasswordResetTokenProviderOptions()
    {
        Name = "PasswordResetTokenProvider";
        TokenLifespan = TimeSpan.FromHours(1);
    }
}