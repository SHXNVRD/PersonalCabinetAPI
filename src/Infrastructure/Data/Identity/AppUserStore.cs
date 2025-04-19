using Domain.Aggregates.UserAggregate;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Infrastructure.Data.Identity;

public class AppUserStore : UserStore<User, IdentityRole<Guid>, AppDbContext, Guid>
{
    public AppUserStore(AppDbContext context, IdentityErrorDescriber? describer = null)
        : base(context, describer)
    { }
}