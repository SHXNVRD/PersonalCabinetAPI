using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppUserStore : UserStore<User, IdentityRole<long>, AppDbContext, long>
    {
        public AppUserStore(AppDbContext context, IdentityErrorDescriber? describer = null)
            : base(context, describer)
        { }
    }
}