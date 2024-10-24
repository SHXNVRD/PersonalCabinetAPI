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
using Application.Users.DTOs;
using FluentResults.Extensions.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Users.Commands.Revoke;
    
namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediatR;

        public AccountController(IMediator mediatR)
        {
            _mediatR = mediatR;
        }

        [HttpPost]
        [ActionName("reg")]
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

        [HttpPost]
        [ActionName("login")]
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

        [HttpPost]
        [Authorize]
        [ActionName("refresh")]
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

        [HttpPut]
        [Authorize]
        [ActionName("revoke")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Revoke([FromBody] RevokeCommand request)
        {
            var result = await _mediatR.Send(request);

            return result.ToActionResult();
        }
    }
}