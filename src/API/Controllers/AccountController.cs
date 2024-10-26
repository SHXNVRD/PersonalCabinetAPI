using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using API.Extensions;
using API.Requests;
using Application.Extensions;
using Application.Users.Commands;
using Application.Users.Commands.Login;
using Application.Users.Commands.RefreshToken;
using Application.Users.Commands.Registration;
using Application.Users.Commands.RevokeRefreshToken;
using Application.Users.DTOs;
using FluentResults.Extensions.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
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
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<AuthResponse>> Registration([FromBody] RegistrationCommand request)
        {
            var result = await _mediatR.Send(request);

            if (result.IsFailed)
                return result.ToUnauthorizedResult();

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

        [HttpPost("tokens/refresh-token")]
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

        [HttpDelete("tokens/refresh-token/{userId}")]
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