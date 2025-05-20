using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data.Identity.TokenProviders;

public class PasswordResetTokenProviderOptions : DataProtectionTokenProviderOptions
{
    public PasswordResetTokenProviderOptions()
    {
        Name = "PasswordResetTokenProvider";
        TokenLifespan = TimeSpan.FromHours(1);
    }
}