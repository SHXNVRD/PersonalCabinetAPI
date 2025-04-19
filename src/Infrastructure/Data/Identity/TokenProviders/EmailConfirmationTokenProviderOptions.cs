using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data.Identity.TokenProviders;

public class EmailConfirmationTokenProviderOptions : DataProtectionTokenProviderOptions
{
    public EmailConfirmationTokenProviderOptions()
    {
        Name = "EmailConfirmationTokenProvider";
        // TODO: Определиться с временем жизни
        // По факту бесполезно, дефолтный провайдер имеет такое же время жизни
        TokenLifespan = TimeSpan.FromDays(1);
    }
}