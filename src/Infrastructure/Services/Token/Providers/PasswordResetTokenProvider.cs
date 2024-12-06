using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.Token.Providers;

public class PasswordResetTokenProvider<TUser>
   : DataProtectorTokenProvider<TUser> where TUser : class
{
   public PasswordResetTokenProvider(
      IDataProtectionProvider dataProtectionProvider,
      IOptions<PasswordResetTokenProviderOptions> options,
      ILogger<DataProtectorTokenProvider<TUser>> logger)
      : base(dataProtectionProvider, options, logger)
   { }
}