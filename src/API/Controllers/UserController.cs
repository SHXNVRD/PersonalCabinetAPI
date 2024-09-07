using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet("{email}")]
        public Task<ActionResult<UserResponse>> GetByEmail([FromRoute] string email)
        {
            throw new Exception();
        }
    }
}