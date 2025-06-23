using Microsoft.AspNetCore.Identity;

namespace Domain.Aggregates.UserAggregate;

public class Role : IdentityRole<Guid>
{
    public const string User = "User";
    public const string Admin = "Admin";

    public static IEnumerable<string> All()
        => [User, Admin];
}