using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AppRoleStore : RoleStore<IdentityRole<Guid>, AppDbContext, Guid>
{
    public AppRoleStore(AppDbContext context, IdentityErrorDescriber? describer = null)
        : base(context, describer)
    { }
}