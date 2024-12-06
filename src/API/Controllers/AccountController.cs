using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using API.Extensions;
using API.Extensions.RequestsExtensions;
using API.Requests;
using Application.DTOs.Emails;
using Application.Extensions;
using Application.Interfaces.Email;
using Application.Users.Commands;
using Application.Users.Commands.CreateEmailConfirmationLink;
using Application.Users.Commands.EmailConfirmation;
using Application.Users.Commands.Login;
using Application.Users.Commands.RefreshToken;
using Application.Users.Commands.Registration;
using Application.Users.Commands.ResetPassword;
using Application.Users.Commands.RevokeRefreshToken;
using Application.Users.Commands.SendPasswordResetLink;
using Application.Users.DTOs;
using FluentResults;
using FluentResults.Extensions.AspNetCore;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharpGrip.FluentValidation.AutoValidation.Shared.Extensions;

namespace API.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediatR;

        public AccountController(IMediator mediatR)
        {
            _mediatR = mediatR;
        }

        [HttpPost("auth/account")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<AuthResponse>> Registration([FromBody] RegistrationCommand request)
        {
            var result = await _mediatR.Send(request);

            if (result.IsFailed)
                return result.ToConflictResult();
            
            return result.ToActionResult();
        }
    
        [HttpPost("auth/account/session")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginCommand request)
        {
            var result = await _mediatR.Send(request);

            if (result.IsFailed)
               return result.ToUnauthorizedResult();

            return result.ToActionResult();
        }
        
        [HttpGet("auth/account/email/confirmation")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
        {
            var command = new EmailConfirmationCommand(email, token);
            var validator = new EmailConfirmationCommandValidator();
            var validationResult = await validator.ValidateAsync(command);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.ToValidationProblemErrors());
            
            var result = await _mediatR.Send(command);

            if (result.IsFailed)
                return result.ToConflictResult();
                    
            return result.ToActionResult();
        }

        [HttpPost("auth/account/email/confirmation-link")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateEmailConfirmationLink([FromBody] CreateEmailConfirmationLinkCommand request)
        {
            var result = await _mediatR.Send(request);

            if (result.IsFailed)
                return result.ToConflictResult();

            return NoContent();
        }

        [HttpPost("auth/account/password/reset")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ResetPasswordAsync([FromBody] ResetPasswordRequest request)
        {
            var result = await _mediatR.Send(request.ToCommand());

            if (result.IsFailed)
                return result.ToNotFoundResult();

            return result.ToActionResult();
        }

        [HttpPost("auth/account/password/reset-link")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> SendPasswordResetLink([FromBody] SendPasswordResetLinkRequest request)
        {
            var result = await _mediatR.Send(request.ToCommand());

            if (result.IsFailed)
                return result.ToNotFoundResult();

            return NoContent();
        }
        
        [HttpPost("auth/tokens/refresh-token")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<AuthResponse>> Refresh([FromBody] RefershTokenRequest request)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized("Access token does not contain user id");
            
            var command = new RefreshTokenCommand(
                UserId: userId,
                RefreshToken: request.RefreshToken
            );
            
            var result = await _mediatR.Send(command);
            
            return result.ToActionResult();
        }

        [HttpDelete("auth/tokens/refresh-token/{userId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> RevokeRefreshToken([FromRoute] string userId)
        {
            var result = await _mediatR.Send(new RevokeRefreshTokenCommand(userId));

            return result.ToActionResult();
        }
    }
}