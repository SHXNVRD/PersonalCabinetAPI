using System.Security.Claims;
using API.Controllers.User.DTOs;
using API.Extensions;
using Application.Users.DTOs;
using Application.Users.Queries.GetAll;
using Application.Users.Queries.GetById;
using Domain.Shared.Errors;
using FluentResults;
using FluentResults.Extensions.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace API.Controllers.User;

[Route("api/users")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IMediator _mediatR;

    public UserController(IMediator mediatR)
    {
        _mediatR = mediatR;
    }

    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<ActionResult<GetUserByIdResponse>> GetCurrent()
    {
        var userIdResult = HttpContext.User.GetUserId();
        if (userIdResult.IsFailed)
            return userIdResult.ToObjectResult(HttpContext);

        var parsedId = Guid.Parse(userIdResult.Value);
        var command = new GetUserByIdQuery
        {
            Id = parsedId
        };
        
        var result = await _mediatR.Send(command);
        
        if (result.IsFailed)
            return result.ToObjectResult(HttpContext);
            
        return result.ToActionResult();
    }
        
    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetUserByIdResponse>> GetById(Guid id)
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

    [HttpGet]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetUsersResponse>> GetAll([FromQuery] GetUsersRequest request)
    {
        var command = GetUsersMapper.ToQuery(request);

        var result = await _mediatR.Send(command);
        
        if (result.IsFailed)
            return result.ToObjectResult(HttpContext);

        return result.ToActionResult();
    }
}