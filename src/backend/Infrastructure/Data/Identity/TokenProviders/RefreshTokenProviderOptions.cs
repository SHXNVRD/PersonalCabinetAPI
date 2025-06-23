using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data.Identity.TokenProviders;

public class RefreshTokenProviderOptions : DataProtectionTokenProviderOptions
{
    public RefreshTokenProviderOptions()
    {
        Name = "RefreshTokenProvider";
        TokenLifespan = TimeSpan.FromDays(90);
    }
}