using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Tokens.Providers
{
    public class EmailConfirmationTokenProviderOptions : DataProtectionTokenProviderOptions
    {
        public EmailConfirmationTokenProviderOptions()
        {
            Name = "EmailDataProtectorTokenProvider";
            TokenLifespan = TimeSpan.FromDays(1);
        }
    }
}