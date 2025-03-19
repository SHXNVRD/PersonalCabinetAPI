using System.Security.Claims;
using API.DTOs.CardController;
using API.Extensions;
using Application.Cards.Commands.Activate;
using Domain.Shared.Errors;
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
        public async Task<ActionResult<ActivateCardResponse>> Activate([FromBody] ActivateCardRequest request)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Result
                    .Fail(new Unauthorized("Access token does not contain user id"))
                    .ToObjectResult(HttpContext);

            var command = ActivateCardMapper.ToCommand(request);
            command.UserId = userId;
            
            var result = await _mediatR.Send(command);

            if (result.IsFailed)
                return result.ToObjectResult(HttpContext);

            return result.ToActionResult();
        }

        [HttpPut("block")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Deactivate([FromBody] BlockCardRequest request)
        {
            var command = DeactivateCardMapper.ToCommand(request);
            var result = await _mediatR.Send(command);

            if (result.IsFailed)
                return result.ToObjectResult(HttpContext);

            return NoContent();
        }
    }
}