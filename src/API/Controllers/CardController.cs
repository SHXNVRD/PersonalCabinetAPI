using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Cards.Commands;
using Application.Cards.Queries;
using Application.DTOs;
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
    public class CardController : ControllerBase
    {
        private readonly IMediator _mediatR;

        public CardController(IMediator mediatR)
        {
            _mediatR = mediatR;
        }

        [HttpPost]
        [ActionName("activate")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<CardActivatedResponse>> Activate([FromBody] ActivateCardCommand request)
        {
            var result = await _mediatR.Send(request);

            return result.ToActionResult();
        }

        [HttpGet]
        [ActionName("get")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<CardResponse>> GetById([FromQuery] GetCardByIdQuery request)
        {
            var result = await _mediatR.Send(request);

            if (result.IsFailed)
                return NotFound(result.Errors);

            return result.ToActionResult();
        }
    }
}