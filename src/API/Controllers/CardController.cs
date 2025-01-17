using System.Security.Claims;
using API.DTOs.CardController;
using API.Extensions;
using API.DTOs;
using Application.Cards.Commands;
using Application.Cards.Commands.Activate;
using Application.Cards.Commands.Deactivate;
using Application.Cards.DTOs;
using Application.Cards.Queries;
using Application.Cards.Queries.GetCardByUserId;
using Application.DTOs;
using Application.Extensions;
using FluentResults;
using FluentResults.Extensions.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/v1/cards")]
    [ApiController]
    public class CardController : ControllerBase
    {
        private readonly IMediator _mediatR;

        public CardController(IMediator mediatR)
        {
            _mediatR = mediatR;
        }

        [HttpPut("activation")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CardActivatedResponse>> Activate([FromBody] ActivateCardRequest request)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (userId == null)
                return Unauthorized("Access token does not contain user id");

            var command = ActivateCardMapper.ToCommand(request);
            command.UserId = userId;
            
            var result = await _mediatR.Send(command);

            if (result.IsFailed)
                return result.ToNotFoundResult();

            return result.ToActionResult();
        }

        [HttpGet("current")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<CardResponse>> GetCurrent()
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized("Access token does not contain user id");

            if (!long.TryParse(userId, out var parsedId))
                return Unauthorized("User Id must be a digit");

            var command = new GetCardByUserIdQuery
            {
                Id = parsedId
            };

            var result = await _mediatR.Send(command);

            if (result.IsFailed)
                return result.ToNotFoundResult();

            return result.ToActionResult();
        }

        [HttpPut("deactivation")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Deactivate([FromBody] DeactivateCardRequest request)
        {
            var command = DeactivateCardMapper.ToCommand(request);
            var result = await _mediatR.Send(command);

            if (result.IsFailed)
                return result.ToNotFoundResult();

            return NoContent();
        }
    }
}