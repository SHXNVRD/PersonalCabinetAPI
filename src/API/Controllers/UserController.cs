using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Extensions;
using Application.Users.Queries.GetById;
using FluentResults;
using FluentResults.Extensions.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediatR;

        public UserController(IMediator mediatR)
        {
            _mediatR = mediatR;
        }

        [HttpGet]
        [ActionName("get")]
        public async Task<ActionResult<UserResponse>> Get()
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized("Access token does not contain user id");

            var result = await _mediatR.Send(new GetUserByIdQuery(userId));

            if (result.IsFailed)
                return result.ToNotFoundResult();
            
            return result.ToActionResult();
        }
    }
}