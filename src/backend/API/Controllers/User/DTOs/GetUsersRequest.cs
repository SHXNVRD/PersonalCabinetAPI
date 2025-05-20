using Application.Users.Queries.GetAll;
using Microsoft.AspNetCore.Mvc;
using Riok.Mapperly.Abstractions;

namespace API.Controllers.User.DTOs;

public record GetUsersRequest(
    [FromQuery(Name = "page")] int Page, 
    [FromQuery(Name = "pageSize")] int PageSize);

[Mapper]
public static partial class GetUsersMapper
{
    public static partial GetUsersQuery ToQuery(GetUsersRequest request);
}