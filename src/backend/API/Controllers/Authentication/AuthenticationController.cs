using API.Controllers.Authentication.DTOs;
using API.Extensions;
using Application.Users.Commands.RevokeRefreshToken;
using Application.Users.DTOs;
using FluentResults.Extensions.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CreateEmailConfirmationLinkMapper = API.Controllers.Authentication.DTOs.CreateEmailConfirmationLinkMapper;
using LoginMapper = API.Controllers.Authentication.DTOs.LoginMapper;
using RefreshTokenMapper = API.Controllers.Authentication.DTOs.RefreshTokenMapper;
using RegistrationMapper = API.Controllers.Authentication.DTOs.RegistrationMapper;
using ResetPasswordMapper = API.Controllers.Authentication.DTOs.ResetPasswordMapper;
using SendPasswordResetLinkMapper = API.Controllers.Authentication.DTOs.SendPasswordResetLinkMapper;

namespace API.Controllers.Authentication;

[Route("api/auth")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthenticationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Registration([FromBody] RegistrationRequest request)
    {
        var command = RegistrationMapper.ToCommand(request);
        var result = await _mediator.Send(command);

        if (result.IsFailed)
            return result.ToObjectResult(HttpContext);

        return NoContent();
    }
    
    [HttpPost("token")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var command = LoginMapper.ToCommand(request);
        var result = await _mediator.Send(command);

        if (result.IsFailed)
            return result.ToObjectResult(HttpContext);

        return result.ToActionResult();
    }
        
    [HttpGet("email/confirm")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ConfirmEmail([FromQuery] ConfirmEmailRequest request)
    {
        var command = ConfirmEmailMapper.ToCommand(request);
        
        var result = await _mediator.Send(command);

        if (result.IsFailed)
            return result.ToObjectResult(HttpContext);
        
        if (request.RedirectUrl is not null)
            return Redirect(request.RedirectUrl);

        return NoContent();
    }
    
    [HttpPost("email/confirmation-link")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CreateEmailConfirmationLink([FromBody] CreateEmailConfirmationLinkRequest request)
    {
        var command = CreateEmailConfirmationLinkMapper.ToCommand(request);
        var result = await _mediator.Send(command);

        if (result.IsFailed)
            return result.ToObjectResult(HttpContext);

        return NoContent();
    }

    [HttpPost("password/reset")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ResetPasswordAsync([FromBody] ResetPasswordRequest request)
    {
        var command = ResetPasswordMapper.ToCommand(request);
        var result = await _mediator.Send(command);

        if (result.IsFailed)
            return result.ToObjectResult(HttpContext);

        return NoContent();
    }

    [HttpPost("password/reset-link")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> SendPasswordResetLink([FromBody] SendPasswordResetLinkRequest request)
    {
        var command = SendPasswordResetLinkMapper.ToCommand(request);
        var result = await _mediator.Send(command);

        if (result.IsFailed)
            return result.ToObjectResult(HttpContext);

        return NoContent();
    }
        
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthResponse>> Refresh([FromBody] RefreshTokenRequest request)
    {
        var command = RefreshTokenMapper.ToCommand(request);
            
        var result = await _mediator.Send(command);

        if (result.IsFailed)
            return result.ToObjectResult(HttpContext);
            
        return result.ToActionResult();
    }

    [HttpDelete("refresh/{userId:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> RevokeRefreshToken([FromRoute] Guid userId)
    {
        var command = new RevokeRefreshTokenCommand
        {
            UserId = userId
        };
            
        var result = await _mediator.Send(command);

        if (result.IsFailed)
            return result.ToObjectResult(HttpContext);

        return NoContent();
    }
}