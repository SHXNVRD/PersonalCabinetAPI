using System.Security.Claims;
using API.Controllers.Authentication.DTOs;
using API.Extensions;
using Application.Users.Commands.EmailConfirmation;
using Application.Users.Commands.RevokeRefreshToken;
using Application.Users.DTOs;
using Domain.Shared.Errors;
using FluentResults;
using FluentResults.Extensions.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharpGrip.FluentValidation.AutoValidation.Shared.Extensions;
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
    private readonly IMediator _mediatR;

    public AuthenticationController(IMediator mediatR)
    {
        _mediatR = mediatR;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Registration([FromBody] RegistrationRequest request)
    {
        var command = RegistrationMapper.ToCommand(request);
        var result = await _mediatR.Send(command);

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
        var result = await _mediatR.Send(command);

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
    public async Task<ActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token, [FromQuery] string? redirectUrl)
    {
        var command = new EmailConfirmationCommand
        {
            Email = email,
            Token = token
        };
            
        var validator = new EmailConfirmationCommandValidator();
        var validationResult = await validator.ValidateAsync(command);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.ToValidationProblemErrors());
            
        var result = await _mediatR.Send(command);

        if (result.IsFailed)
            return result.ToObjectResult(HttpContext);

        if (Uri.IsWellFormedUriString(redirectUrl, UriKind.Absolute))
            return Redirect(redirectUrl);

        return NoContent();
    }
    
    [HttpPost("email/confirmation-link")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CreateEmailConfirmationLink([FromBody] CreateEmailConfirmationLinkRequest request)
    {
        var command = CreateEmailConfirmationLinkMapper.ToCommand(request);
        var result = await _mediatR.Send(command);

        if (result.IsFailed)
            return result.ToObjectResult(HttpContext);

        return NoContent();
    }

    [HttpPost("password/reset")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ResetPasswordAsync([FromBody] ResetPasswordRequest request)
    {
        var command = ResetPasswordMapper.ToCommand(request);
        var result = await _mediatR.Send(command);

        if (result.IsFailed)
            return result.ToObjectResult(HttpContext);

        return NoContent();
    }

    [HttpPost("password/reset-link")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> SendPasswordResetLink([FromBody] SendPasswordResetLinkRequest request)
    {
        var command = SendPasswordResetLinkMapper.ToCommand(request);
        var result = await _mediatR.Send(command);

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
            
        var result = await _mediatR.Send(command);

        if (result.IsFailed)
            return result.ToObjectResult(HttpContext);
            
        return result.ToActionResult();
    }

    [HttpDelete("refresh/{userId}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> RevokeRefreshToken([FromRoute] string userId)
    {
        var command = new RevokeRefreshTokenCommand
        {
            UserId = userId
        };
            
        var result = await _mediatR.Send(command);

        if (result.IsFailed)
            return result.ToObjectResult(HttpContext);

        return NoContent();
    }
}