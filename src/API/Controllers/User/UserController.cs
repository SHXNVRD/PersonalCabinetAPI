using System.Security.Claims;
using API.Extensions;
using Application.Users.DTOs;
using Application.Users.Queries.GetById;
using Domain.Shared.Errors;
using FluentResults;
using FluentResults.Extensions.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.User;

[Route("api/v1/users")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IMediator _mediatR;

    public UserController(IMediator mediatR)
    {
        _mediatR = mediatR;
    }

    [HttpGet("current")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetUserByIdResponse>> GetCurrent()
    {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
            return Result
                .Fail(new Unauthorized("Access token does not contain user id"))
                .ToObjectResult(HttpContext);

        var command = new GetUserByIdQuery
        {
            Id = userId
        };
        var result = await _mediatR.Send(command);

        if (result.IsFailed)
            return result.ToObjectResult(HttpContext);
            
        return result.ToActionResult();
    }
        
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetUserByIdResponse>> GetById(string id)
    {
        var command = new GetUserByIdQuery
        {
            Id = id
        };
        var result = await _mediatR.Send(command);

        if (result.IsFailed)
            return result.ToObjectResult(HttpContext);
            
        return result.ToActionResult();
    }
}