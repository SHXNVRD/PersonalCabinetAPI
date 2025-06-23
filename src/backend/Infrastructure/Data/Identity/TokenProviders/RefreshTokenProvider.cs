using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Data.Identity.TokenProviders;

public class RefreshTokenProvider<TUser>
    : DataProtectorTokenProvider<TUser> where TUser : class
{
    public RefreshTokenProvider(
        IDataProtectionProvider dataProtectionProvider, 
        IOptions<RefreshTokenProviderOptions> options, 
        ILogger<DataProtectorTokenProvider<TUser>> logger) 
        : base(dataProtectionProvider, options, logger)
    { }
}