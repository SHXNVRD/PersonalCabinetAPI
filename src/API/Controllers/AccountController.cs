using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Users.Commands.Registration;
using FluentResults.Extensions.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult<AuthResponse>> Registration([FromBody] RegistrationCommand request)
        {
            var result = await _mediatR.Send(request);

            if (result.IsFailed)
                return StatusCode(500, result);

            return result.ToActionResult();
        }
    }
}