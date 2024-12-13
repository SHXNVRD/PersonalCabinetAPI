using System.Security.Claims;
using API.DTOs.AccountController;
using API.Extensions;
using Application.Users.Commands.EmailConfirmation;
using Application.Users.Commands.RevokeRefreshToken;
using Application.Users.DTOs;
using FluentResults.Extensions.AspNetCore;
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
        public async Task<ActionResult<AuthResponse>> Registration([FromBody] RegistrationRequest request)
        {
            var command = RegistrationMapper.ToCommand(request);
            var result = await _mediatR.Send(command);

            if (result.IsFailed)
                return result.ToConflictResult();
            
            return result.ToActionResult();
        }
    
        [HttpPost("auth/account/session")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            var command = LoginMapper.ToCommand(request);
            var result = await _mediatR.Send(command);

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
                return result.ToConflictResult();
                    
            return result.ToActionResult();
        }

        [HttpPost("auth/account/email/confirmation-link")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateEmailConfirmationLink([FromBody] CreateEmailConfirmationLinkRequest request)
        {
            var command = CreateEmailConfirmationLinkMapper.ToCommand(request);
            var result = await _mediatR.Send(command);

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
            var command = ResetPasswordMapper.ToCommand(request);
            var result = await _mediatR.Send(command);

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
            var command = SendPasswordResetLinkMapper.ToCommand(request);
            var result = await _mediatR.Send(command);

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
        public async Task<ActionResult<AuthResponse>> Refresh([FromBody] RefreshTokenRequest request)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized("Access token does not contain user id");

            var command = RefreshTokenMapper.ToCommand(request);
            command.UserId = userId;
            
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
            var command = new RevokeRefreshTokenCommand
            {
                UserId = userId
            };
            
            var result = await _mediatR.Send(command);

            return result.ToActionResult();
        }
    }
}