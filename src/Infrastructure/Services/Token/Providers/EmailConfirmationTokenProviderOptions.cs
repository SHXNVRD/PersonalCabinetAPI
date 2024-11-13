using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services.Token.Providers
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