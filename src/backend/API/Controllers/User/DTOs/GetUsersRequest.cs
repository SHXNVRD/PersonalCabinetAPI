using Application.Users.Queries.GetAll;
using Riok.Mapperly.Abstractions;

namespace API.Controllers.User.DTOs;

public record GetUsersRequest(int Page, int PageSize);

[Mapper]
public static partial class GetUsersMapper
{
    public static partial GetUsersQuery ToQuery(GetUsersRequest request);
}